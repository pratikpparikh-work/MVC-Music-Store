using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MvcMusicStore.Application.DTOs;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Web.Controllers;
using Xunit;

namespace MvcMusicStore.Tests.Controllers
{
    /// <summary>
    /// Unit tests for HomeController.
    /// Tests the migration from legacy synchronous to async patterns.
    /// </summary>
    public class HomeControllerTests
    {
        private readonly Mock<IAlbumService> _mockAlbumService;
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockAlbumService = new Mock<IAlbumService>();
            _mockLogger = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_mockAlbumService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithTopSellingAlbums()
        {
            // Arrange
            var albums = new List<AlbumDto>
            {
                new AlbumDto { AlbumId = 1, Title = "Test Album 1", Price = 9.99M },
                new AlbumDto { AlbumId = 2, Title = "Test Album 2", Price = 8.99M }
            };
            _mockAlbumService.Setup(s => s.GetTopSellingAlbumsAsync(5))
                .ReturnsAsync(albums);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<AlbumDto>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Index_CallsGetTopSellingAlbums_WithCount5()
        {
            // Arrange
            _mockAlbumService.Setup(s => s.GetTopSellingAlbumsAsync(5))
                .ReturnsAsync(new List<AlbumDto>());

            // Act
            await _controller.Index();

            // Assert
            _mockAlbumService.Verify(s => s.GetTopSellingAlbumsAsync(5), Times.Once);
        }
    }

    /// <summary>
    /// Unit tests for StoreController.
    /// </summary>
    public class StoreControllerTests
    {
        private readonly Mock<IGenreService> _mockGenreService;
        private readonly Mock<IAlbumService> _mockAlbumService;
        private readonly Mock<ILogger<StoreController>> _mockLogger;
        private readonly StoreController _controller;

        public StoreControllerTests()
        {
            _mockGenreService = new Mock<IGenreService>();
            _mockAlbumService = new Mock<IAlbumService>();
            _mockLogger = new Mock<ILogger<StoreController>>();
            _controller = new StoreController(_mockGenreService.Object, _mockAlbumService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithGenres()
        {
            // Arrange
            var genres = new List<GenreDto>
            {
                new GenreDto { GenreId = 1, Name = "Rock" },
                new GenreDto { GenreId = 2, Name = "Jazz" }
            };
            _mockGenreService.Setup(s => s.GetAllGenresAsync()).ReturnsAsync(genres);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<GenreDto>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Browse_ReturnsNotFound_WhenGenreDoesNotExist()
        {
            // Arrange
            _mockGenreService.Setup(s => s.GetGenreWithAlbumsAsync("NonExistent"))
                .ReturnsAsync((GenreDto?)null);

            // Act
            var result = await _controller.Browse("NonExistent");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Browse_ReturnsViewResult_WithGenreAlbums()
        {
            // Arrange
            var genre = new GenreDto
            {
                GenreId = 1,
                Name = "Rock",
                Albums = new List<AlbumDto>
                {
                    new AlbumDto { AlbumId = 1, Title = "Test Album" }
                }
            };
            _mockGenreService.Setup(s => s.GetGenreWithAlbumsAsync("Rock")).ReturnsAsync(genre);

            // Act
            var result = await _controller.Browse("Rock");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<GenreDto>(viewResult.Model);
            Assert.Equal("Rock", model.Name);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenAlbumDoesNotExist()
        {
            // Arrange
            _mockAlbumService.Setup(s => s.GetAlbumByIdAsync(999))
                .ReturnsAsync((AlbumDto?)null);

            // Act
            var result = await _controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithAlbum()
        {
            // Arrange
            var album = new AlbumDto { AlbumId = 1, Title = "Test Album", Price = 9.99M };
            _mockAlbumService.Setup(s => s.GetAlbumByIdAsync(1)).ReturnsAsync(album);

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AlbumDto>(viewResult.Model);
            Assert.Equal("Test Album", model.Title);
        }
    }

    /// <summary>
    /// Unit tests for ShoppingCartController.
    /// Tests session-based cart management (replaces legacy HttpContext.Session).
    /// Uses a real in-memory session implementation to avoid extension method mocking issues.
    /// </summary>
    public class ShoppingCartControllerTests
    {
        private readonly Mock<IShoppingCartService> _mockCartService;
        private readonly Mock<IAlbumService> _mockAlbumService;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<ILogger<ShoppingCartController>> _mockLogger;
        private readonly ShoppingCartController _controller;

        public ShoppingCartControllerTests()
        {
            _mockCartService = new Mock<IShoppingCartService>();
            _mockAlbumService = new Mock<IAlbumService>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockLogger = new Mock<ILogger<ShoppingCartController>>();

            // Use a real TestSession implementation to support extension methods
            var testSession = new TestSession();
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.Session).Returns(testSession);

            // Setup User.Identity for cart ID resolution
            var mockIdentity = new Mock<System.Security.Principal.IIdentity>();
            mockIdentity.Setup(i => i.Name).Returns((string?)null);
            mockIdentity.Setup(i => i.IsAuthenticated).Returns(false);
            var mockPrincipal = new Mock<System.Security.Claims.ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity).Returns(mockIdentity.Object);
            mockHttpContext.Setup(c => c.User).Returns(mockPrincipal.Object);

            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

            _controller = new ShoppingCartController(
                _mockCartService.Object,
                _mockAlbumService.Object,
                _mockHttpContextAccessor.Object,
                _mockLogger.Object);

            // Setup controller context
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithShoppingCart()
        {
            // Arrange
            var cart = new ShoppingCartDto
            {
                CartItems = new List<CartItemDto>(),
                CartTotal = 0
            };
            _mockCartService.Setup(s => s.GetCartAsync(It.IsAny<string>())).ReturnsAsync(cart);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<ShoppingCartDto>(viewResult.Model);
        }

        [Fact]
        public async Task AddToCart_ReturnsNotFound_WhenAlbumDoesNotExist()
        {
            // Arrange
            _mockAlbumService.Setup(s => s.GetAlbumByIdAsync(999))
                .ReturnsAsync((AlbumDto?)null);

            // Act
            var result = await _controller.AddToCart(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RemoveFromCart_ReturnsJsonResult()
        {
            // Arrange
            _mockCartService.Setup(s => s.GetAlbumTitleByCartRecordIdAsync(It.IsAny<string>(), 1))
                .ReturnsAsync("Test Album");
            _mockCartService.Setup(s => s.RemoveFromCartAsync(It.IsAny<string>(), 1))
                .ReturnsAsync(0);
            _mockCartService.Setup(s => s.GetCartTotalAsync(It.IsAny<string>()))
                .ReturnsAsync(0M);
            _mockCartService.Setup(s => s.GetCartCountAsync(It.IsAny<string>()))
                .ReturnsAsync(0);

            // Act
            var result = await _controller.RemoveFromCart(1);

            // Assert
            Assert.IsType<JsonResult>(result);
        }
    }

    /// <summary>
    /// Test implementation of ISession that supports extension methods.
    /// Needed because ISession.GetString/SetString are extension methods that cannot be mocked directly.
    /// </summary>
    public class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = new();

        public bool IsAvailable => true;
        public string Id => Guid.NewGuid().ToString();
        public IEnumerable<string> Keys => _store.Keys;

        public void Clear() => _store.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _store.Remove(key);
        public void Set(string key, byte[] value) => _store[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value!);
    }
}
