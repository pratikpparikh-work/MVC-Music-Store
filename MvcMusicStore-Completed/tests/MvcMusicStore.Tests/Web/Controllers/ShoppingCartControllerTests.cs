using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MvcMusicStore.Application.DTOs;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Web.Controllers;
using Xunit;

namespace MvcMusicStore.Tests.Web.Controllers
{
    /// <summary>
    /// A simple in-memory ISession implementation for testing.
    /// </summary>
    public class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = new();

        public bool IsAvailable => true;
        public string Id => "test-session-id";
        public IEnumerable<string> Keys => _store.Keys;

        public void Clear() => _store.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _store.Remove(key);

        public void Set(string key, byte[] value) => _store[key] = value;

        public bool TryGetValue(string key, out byte[] value)
        {
            if (_store.TryGetValue(key, out var v))
            {
                value = v;
                return true;
            }
            value = Array.Empty<byte>();
            return false;
        }
    }

    public class ShoppingCartControllerTests
    {
        private readonly Mock<IShoppingCartService> _cartServiceMock;
        private readonly Mock<IAlbumService> _albumServiceMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<ILogger<ShoppingCartController>> _loggerMock;
        private readonly ShoppingCartController _controller;
        private readonly TestSession _session;
        private readonly Mock<HttpContext> _httpContextMock;

        public ShoppingCartControllerTests()
        {
            _cartServiceMock = new Mock<IShoppingCartService>();
            _albumServiceMock = new Mock<IAlbumService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _loggerMock = new Mock<ILogger<ShoppingCartController>>();

            _session = new TestSession();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_session);

            // Setup User.Identity
            var identityMock = new Mock<System.Security.Principal.IIdentity>();
            identityMock.Setup(i => i.Name).Returns((string?)null);
            var claimsPrincipalMock = new Mock<System.Security.Claims.ClaimsPrincipal>();
            claimsPrincipalMock.Setup(p => p.Identity).Returns(identityMock.Object);
            _httpContextMock.Setup(c => c.User).Returns(claimsPrincipalMock.Object);

            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(_httpContextMock.Object);

            _controller = new ShoppingCartController(
                _cartServiceMock.Object,
                _albumServiceMock.Object,
                _httpContextAccessorMock.Object,
                _loggerMock.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContextMock.Object
            };
        }

        private void SetupSessionCartId(string cartId)
        {
            // Use the real ISession Set method via extension method bytes
            var bytes = System.Text.Encoding.UTF8.GetBytes(cartId);
            _session.Set("CartId", bytes);
        }

        [Fact]
        public async Task Index_ReturnsViewWithCart()
        {
            // Arrange
            SetupSessionCartId("test-cart");
            var cartDto = new ShoppingCartDto
            {
                CartItems = new List<CartItemDto> { new CartItemDto { RecordId = 1, AlbumTitle = "Test" } },
                CartTotal = 9.99m
            };
            _cartServiceMock.Setup(s => s.GetCartAsync(It.IsAny<string>())).ReturnsAsync(cartDto);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<ShoppingCartDto>(viewResult.Model);
        }

        [Fact]
        public async Task AddToCart_ExistingAlbum_RedirectsToIndex()
        {
            // Arrange
            SetupSessionCartId("test-cart");
            var album = new AlbumDto { AlbumId = 1, Title = "Test Album" };
            _albumServiceMock.Setup(s => s.GetAlbumByIdAsync(1)).ReturnsAsync(album);
            _cartServiceMock.Setup(s => s.AddToCartAsync(It.IsAny<string>(), 1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddToCart(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task AddToCart_NonExistingAlbum_ReturnsNotFound()
        {
            // Arrange
            _albumServiceMock.Setup(s => s.GetAlbumByIdAsync(999)).ReturnsAsync((AlbumDto?)null);

            // Act
            var result = await _controller.AddToCart(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RemoveFromCart_ValidItem_ReturnsJsonResult()
        {
            // Arrange
            SetupSessionCartId("test-cart");
            _cartServiceMock.Setup(s => s.GetAlbumTitleByCartRecordIdAsync(It.IsAny<string>(), 1)).ReturnsAsync("Test Album");
            _cartServiceMock.Setup(s => s.RemoveFromCartAsync(It.IsAny<string>(), 1)).ReturnsAsync(2);
            _cartServiceMock.Setup(s => s.GetCartTotalAsync(It.IsAny<string>())).ReturnsAsync(19.98m);
            _cartServiceMock.Setup(s => s.GetCartCountAsync(It.IsAny<string>())).ReturnsAsync(2);

            // Act
            var result = await _controller.RemoveFromCart(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);
        }

        [Fact]
        public async Task RemoveFromCart_NullAlbumTitle_UsesDefaultMessage()
        {
            // Arrange
            SetupSessionCartId("test-cart");
            _cartServiceMock.Setup(s => s.GetAlbumTitleByCartRecordIdAsync(It.IsAny<string>(), 1)).ReturnsAsync((string?)null);
            _cartServiceMock.Setup(s => s.RemoveFromCartAsync(It.IsAny<string>(), 1)).ReturnsAsync(0);
            _cartServiceMock.Setup(s => s.GetCartTotalAsync(It.IsAny<string>())).ReturnsAsync(0m);
            _cartServiceMock.Setup(s => s.GetCartCountAsync(It.IsAny<string>())).ReturnsAsync(0);

            // Act
            var result = await _controller.RemoveFromCart(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);
        }

        [Fact]
        public async Task Index_NoSessionCartId_CreatesNewCartId()
        {
            // Arrange - no session cart id set
            var cartDto = new ShoppingCartDto { CartItems = new List<CartItemDto>(), CartTotal = 0m };
            _cartServiceMock.Setup(s => s.GetCartAsync(It.IsAny<string>())).ReturnsAsync(cartDto);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }
    }
}
