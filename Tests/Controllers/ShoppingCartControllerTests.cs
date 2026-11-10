using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using MvcMusicStore.Controllers;
using MvcMusicStore.Data;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Controllers
{
    public class ShoppingCartControllerTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ApplicationDbContext _context;

        public ShoppingCartControllerTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;
            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();
            SeedData();
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }

        private void SeedData()
        {
            var genre = new Genre { Name = "Rock" };
            var artist = new Artist { Name = "Test Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            _context.SaveChanges();

            _context.Albums.Add(new Album
            {
                Title = "Test Album",
                Price = 8.99m,
                GenreId = genre.GenreId,
                ArtistId = artist.ArtistId
            });
            _context.SaveChanges();
        }

        private (ShoppingCart cart, Mock<HttpContext> mockHttpContext) CreateCartWithMockContext(string cartId = "test-cart-id")
        {
            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new Mock<ISession>();
            byte[] cartIdBytes = System.Text.Encoding.UTF8.GetBytes(cartId);
            mockSession.Setup(s => s.TryGetValue(ShoppingCart.CartSessionKey, out cartIdBytes)).Returns(true);
            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);
            mockHttpContext.Setup(c => c.User).Returns(new System.Security.Claims.ClaimsPrincipal());
            var cart = ShoppingCart.GetCart(mockHttpContext.Object, _context);
            return (cart, mockHttpContext);
        }

        [Fact]
        public async Task AddToCart_ReturnsNotFound_WhenAlbumDoesNotExist()
        {
            // Arrange
            var (cart, _) = CreateCartWithMockContext();
            var controller = new ShoppingCartController(_context, cart);

            // Act
            var result = await controller.AddToCart(9999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            controller.Dispose();
        }

        [Fact]
        public async Task AddToCart_RedirectsToIndex_WhenAlbumExists()
        {
            // Arrange
            var album = await _context.Albums.FirstAsync();
            var (cart, _) = CreateCartWithMockContext();
            var controller = new ShoppingCartController(_context, cart);

            // Act
            var result = await controller.AddToCart(album.AlbumId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            controller.Dispose();
        }

        [Fact]
        public void Index_ReturnsViewResult_WithShoppingCartViewModel()
        {
            // Arrange
            var (cart, _) = CreateCartWithMockContext();
            var controller = new ShoppingCartController(_context, cart);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
            controller.Dispose();
        }

        [Fact]
        public async Task RemoveFromCart_ReturnsNotFound_WhenCartItemDoesNotExist()
        {
            // Arrange
            var (cart, _) = CreateCartWithMockContext();
            var controller = new ShoppingCartController(_context, cart);

            // Act
            var result = await controller.RemoveFromCart(9999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            controller.Dispose();
        }

        [Fact]
        public async Task RemoveFromCart_ReturnsJsonResult_WhenCartItemExists()
        {
            // Arrange
            var album = await _context.Albums.FirstAsync();
            var (cart, _) = CreateCartWithMockContext();
            cart.AddToCart(album);
            var cartItem = cart.GetCartItems()[0];

            var controller = new ShoppingCartController(_context, cart);

            // Act
            var result = await controller.RemoveFromCart(cartItem.RecordId);

            // Assert
            Assert.IsType<JsonResult>(result);
            controller.Dispose();
        }
    }
}
