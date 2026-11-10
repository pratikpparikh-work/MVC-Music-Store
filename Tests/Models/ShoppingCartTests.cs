using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using MvcMusicStore.Data;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Models
{
    public class ShoppingCartTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ApplicationDbContext _context;
        private readonly Mock<HttpContext> _mockHttpContext;
        private readonly Mock<ISession> _mockSession;
        private readonly ShoppingCart _cart;
        private readonly string _cartId = "test-cart-id";

        public ShoppingCartTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;
            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            _mockSession = new Mock<ISession>();
            _mockHttpContext = new Mock<HttpContext>();

            // Setup session to return our test cart ID
            byte[] cartIdBytes = System.Text.Encoding.UTF8.GetBytes(_cartId);
            _mockSession.Setup(s => s.TryGetValue(ShoppingCart.CartSessionKey, out cartIdBytes)).Returns(true);
            _mockHttpContext.Setup(c => c.Session).Returns(_mockSession.Object);
            _mockHttpContext.Setup(c => c.User).Returns(new System.Security.Claims.ClaimsPrincipal());

            _cart = ShoppingCart.GetCart(_mockHttpContext.Object, _context);
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }

        private Album SeedAlbum(string title = "Test Album", decimal price = 8.99m)
        {
            var genre = new Genre { Name = $"Genre-{Guid.NewGuid()}" };
            var artist = new Artist { Name = $"Artist-{Guid.NewGuid()}" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            _context.SaveChanges();

            var album = new Album
            {
                Title = title,
                Price = price,
                GenreId = genre.GenreId,
                ArtistId = artist.ArtistId
            };
            _context.Albums.Add(album);
            _context.SaveChanges();
            return album;
        }

        [Fact]
        public void GetCart_ReturnsShoppingCartInstance()
        {
            // Assert
            Assert.NotNull(_cart);
        }

        [Fact]
        public void CartSessionKey_HasCorrectValue()
        {
            // Assert
            Assert.Equal("CartId", ShoppingCart.CartSessionKey);
        }

        [Fact]
        public void AddToCart_AddsNewItem_WhenItemNotInCart()
        {
            // Arrange
            var album = SeedAlbum("New Album");

            // Act
            _cart.AddToCart(album);

            // Assert
            var cartItems = _cart.GetCartItems();
            Assert.Single(cartItems);
            Assert.Equal(album.AlbumId, cartItems[0].AlbumId);
            Assert.Equal(1, cartItems[0].Count);
        }

        [Fact]
        public void AddToCart_IncrementsCount_WhenItemAlreadyInCart()
        {
            // Arrange
            var album = SeedAlbum("Existing Album");
            _cart.AddToCart(album);

            // Act
            _cart.AddToCart(album);

            // Assert
            var cartItems = _cart.GetCartItems();
            Assert.Single(cartItems);
            Assert.Equal(2, cartItems[0].Count);
        }

        [Fact]
        public void GetCartItems_ReturnsEmptyList_WhenCartIsEmpty()
        {
            // Act
            var items = _cart.GetCartItems();

            // Assert
            Assert.NotNull(items);
            Assert.Empty(items);
        }

        [Fact]
        public void GetCount_ReturnsZero_WhenCartIsEmpty()
        {
            // Act
            var count = _cart.GetCount();

            // Assert
            Assert.Equal(0, count);
        }

        [Fact]
        public void GetCount_ReturnsCorrectCount_AfterAddingItems()
        {
            // Arrange
            var album1 = SeedAlbum("Album 1", 8.99m);
            var album2 = SeedAlbum("Album 2", 9.99m);
            _cart.AddToCart(album1);
            _cart.AddToCart(album1);
            _cart.AddToCart(album2);

            // Act
            var count = _cart.GetCount();

            // Assert
            Assert.Equal(3, count);
        }

        [Fact]
        public void GetTotal_ReturnsZero_WhenCartIsEmpty()
        {
            // Act
            var total = _cart.GetTotal();

            // Assert
            Assert.Equal(0m, total);
        }

        [Fact]
        public void EmptyCart_RemovesAllItems()
        {
            // Arrange
            var album = SeedAlbum("Album to Remove");
            _cart.AddToCart(album);
            Assert.Single(_cart.GetCartItems());

            // Act
            _cart.EmptyCart();

            // Assert
            Assert.Empty(_cart.GetCartItems());
        }

        [Fact]
        public void RemoveFromCart_DecrementsCount_WhenCountGreaterThanOne()
        {
            // Arrange
            var album = SeedAlbum("Album to Decrement");
            _cart.AddToCart(album);
            _cart.AddToCart(album);
            var cartItem = _cart.GetCartItems()[0];

            // Act
            var itemCount = _cart.RemoveFromCart(cartItem.RecordId);

            // Assert
            Assert.Equal(1, itemCount);
        }

        [Fact]
        public void RemoveFromCart_RemovesItem_WhenCountIsOne()
        {
            // Arrange
            var album = SeedAlbum("Album to Remove");
            _cart.AddToCart(album);
            var cartItem = _cart.GetCartItems()[0];

            // Act
            var itemCount = _cart.RemoveFromCart(cartItem.RecordId);

            // Assert
            Assert.Equal(0, itemCount);
            Assert.Empty(_cart.GetCartItems());
        }

        [Fact]
        public void MigrateCart_UpdatesCartId_ToUserName()
        {
            // Arrange
            var album = SeedAlbum("Album to Migrate");
            _cart.AddToCart(album);
            var userName = "testuser@example.com";

            // Act
            _cart.MigrateCart(userName);

            // Assert
            var migratedItems = _context.Carts.Where(c => c.CartId == userName).ToList();
            Assert.Single(migratedItems);
        }

        [Fact]
        public void GetCartId_ReturnsExistingCartId_WhenSessionHasValue()
        {
            // Act
            var cartId = _cart.GetCartId();

            // Assert
            Assert.Equal(_cartId, cartId);
        }

        [Fact]
        public void GetCartId_CreatesNewCartId_WhenSessionIsEmpty()
        {
            // Arrange
            var mockSession = new Mock<ISession>();
            var mockHttpContext = new Mock<HttpContext>();
            byte[]? nullBytes = null;
            mockSession.Setup(s => s.TryGetValue(ShoppingCart.CartSessionKey, out nullBytes)).Returns(false);
            mockSession.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()));
            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);
            mockHttpContext.Setup(c => c.User).Returns(new System.Security.Claims.ClaimsPrincipal());

            var newCart = ShoppingCart.GetCart(mockHttpContext.Object, _context);

            // Act
            var cartId = newCart.GetCartId();

            // Assert
            Assert.NotNull(cartId);
            Assert.NotEmpty(cartId);
        }

        [Fact]
        public void CreateOrder_ReturnsOrderId_AndEmptiesCart()
        {
            // Arrange
            var album = SeedAlbum("Order Album", 10.00m);
            _cart.AddToCart(album);

            var order = new Order
            {
                Username = "testuser",
                FirstName = "Test",
                LastName = "User",
                Address = "123 Main St",
                City = "Seattle",
                State = "WA",
                PostalCode = "98101",
                Country = "USA",
                Phone = "555-1234",
                Email = "test@example.com",
                OrderDate = DateTime.Now
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            // Act
            var orderId = _cart.CreateOrder(order);

            // Assert
            Assert.Equal(order.OrderId, orderId);
            Assert.Empty(_cart.GetCartItems());
            Assert.Equal(10.00m, order.Total);
        }

        [Fact]
        public void GetTotal_ReturnsCorrectTotal_AfterAddingItems()
        {
            // Arrange
            var album = SeedAlbum("Priced Album", 9.99m);
            _cart.AddToCart(album);
            _cart.AddToCart(album);

            // Act
            var total = _cart.GetTotal();

            // Assert
            Assert.Equal(19.98m, total);
        }
    }
}
