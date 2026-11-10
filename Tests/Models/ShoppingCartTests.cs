using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using MvcMusicStore.Data;
using MvcMusicStore.Models;
using MvcMusicStore.Tests.Helpers;

namespace MvcMusicStore.Tests.Models
{
    public class ShoppingCartTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly TestApplicationDbContext _context;
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
            _context = new TestApplicationDbContext(options);
            _context.Database.EnsureCreated();

            _mockSession = new Mock<ISession>();
            _mockHttpContext = new Mock<HttpContext>();
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
            var album = new Album { Title = title, Price = price, GenreId = genre.GenreId, ArtistId = artist.ArtistId };
            _context.Albums.Add(album);
            _context.SaveChanges();
            return album;
        }

        [Fact]
        public void GetCart_ReturnsShoppingCartInstance()
        {
            Assert.NotNull(_cart);
        }

        [Fact]
        public void CartSessionKey_HasCorrectValue()
        {
            Assert.Equal("CartId", ShoppingCart.CartSessionKey);
        }

        [Fact]
        public void AddToCart_AddsNewItem_WhenItemNotInCart()
        {
            var album = SeedAlbum("New Album");
            _cart.AddToCart(album);
            var cartItems = _cart.GetCartItems();
            Assert.Single(cartItems);
            Assert.Equal(album.AlbumId, cartItems[0].AlbumId);
            Assert.Equal(1, cartItems[0].Count);
        }

        [Fact]
        public void AddToCart_IncrementsCount_WhenItemAlreadyInCart()
        {
            var album = SeedAlbum("Existing Album");
            _cart.AddToCart(album);
            _cart.AddToCart(album);
            var cartItems = _cart.GetCartItems();
            Assert.Single(cartItems);
            Assert.Equal(2, cartItems[0].Count);
        }

        [Fact]
        public void GetCartItems_ReturnsEmptyList_WhenCartIsEmpty()
        {
            var items = _cart.GetCartItems();
            Assert.NotNull(items);
            Assert.Empty(items);
        }

        [Fact]
        public void GetCount_ReturnsZero_WhenCartIsEmpty()
        {
            Assert.Equal(0, _cart.GetCount());
        }

        [Fact]
        public void GetCount_ReturnsCorrectCount_AfterAddingItems()
        {
            var album1 = SeedAlbum("Album 1", 8.99m);
            var album2 = SeedAlbum("Album 2", 9.99m);
            _cart.AddToCart(album1);
            _cart.AddToCart(album1);
            _cart.AddToCart(album2);
            Assert.Equal(3, _cart.GetCount());
        }

        [Fact]
        public void GetTotal_ReturnsZero_WhenCartIsEmpty()
        {
            Assert.Equal(0m, _cart.GetTotal());
        }

        [Fact]
        public void EmptyCart_RemovesAllItems()
        {
            var album = SeedAlbum("Album to Remove");
            _cart.AddToCart(album);
            Assert.Single(_cart.GetCartItems());
            _cart.EmptyCart();
            Assert.Empty(_cart.GetCartItems());
        }

        [Fact]
        public void RemoveFromCart_DecrementsCount_WhenCountGreaterThanOne()
        {
            var album = SeedAlbum("Album to Decrement");
            _cart.AddToCart(album);
            _cart.AddToCart(album);
            var cartItem = _cart.GetCartItems()[0];
            var itemCount = _cart.RemoveFromCart(cartItem.RecordId);
            Assert.Equal(1, itemCount);
        }

        [Fact]
        public void RemoveFromCart_RemovesItem_WhenCountIsOne()
        {
            var album = SeedAlbum("Album to Remove");
            _cart.AddToCart(album);
            var cartItem = _cart.GetCartItems()[0];
            var itemCount = _cart.RemoveFromCart(cartItem.RecordId);
            Assert.Equal(0, itemCount);
            Assert.Empty(_cart.GetCartItems());
        }

        [Fact]
        public void MigrateCart_UpdatesCartId_ToUserName()
        {
            var album = SeedAlbum("Album to Migrate");
            _cart.AddToCart(album);
            var userName = "testuser@example.com";
            _cart.MigrateCart(userName);
            var migratedItems = _context.Carts.Where(c => c.CartId == userName).ToList();
            Assert.Single(migratedItems);
        }

        [Fact]
        public void GetCartId_ReturnsExistingCartId_WhenSessionHasValue()
        {
            var cartId = _cart.GetCartId();
            Assert.Equal(_cartId, cartId);
        }

        [Fact]
        public void GetCartId_CreatesNewCartId_WhenSessionIsEmpty()
        {
            var mockSession = new Mock<ISession>();
            var mockHttpContext = new Mock<HttpContext>();
            byte[]? nullBytes = null;
            mockSession.Setup(s => s.TryGetValue(ShoppingCart.CartSessionKey, out nullBytes)).Returns(false);
            mockSession.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()));
            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);
            mockHttpContext.Setup(c => c.User).Returns(new System.Security.Claims.ClaimsPrincipal());
            var newCart = ShoppingCart.GetCart(mockHttpContext.Object, _context);
            var cartId = newCart.GetCartId();
            Assert.NotNull(cartId);
            Assert.NotEmpty(cartId);
        }

        [Fact]
        public void CreateOrder_ReturnsOrderId_AndEmptiesCart()
        {
            var album = SeedAlbum("Order Album", 10.00m);
            _cart.AddToCart(album);
            var order = new Order
            {
                Username = "testuser", FirstName = "Test", LastName = "User",
                Address = "123 Main St", City = "Seattle", State = "WA",
                PostalCode = "98101", Country = "USA", Phone = "555-1234",
                Email = "test@example.com", OrderDate = DateTime.Now
            };
            _context.Orders.Add(order);
            _context.SaveChanges();
            var orderId = _cart.CreateOrder(order);
            Assert.Equal(order.OrderId, orderId);
            Assert.Empty(_cart.GetCartItems());
            Assert.Equal(10.00m, order.Total);
        }

        [Fact]
        public void GetTotal_ReturnsCorrectTotal_AfterAddingItems()
        {
            var album = SeedAlbum("Priced Album", 9.99m);
            _cart.AddToCart(album);
            _cart.AddToCart(album);
            Assert.Equal(19.98m, _cart.GetTotal());
        }
    }
}
