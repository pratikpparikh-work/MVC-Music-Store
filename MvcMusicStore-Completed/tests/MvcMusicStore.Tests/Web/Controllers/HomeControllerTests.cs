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
    public class HomeControllerTests
    {
        private readonly Mock<IAlbumService> _albumServiceMock;
        private readonly Mock<ILogger<HomeController>> _loggerMock;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _albumServiceMock = new Mock<IAlbumService>();
            _loggerMock = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_albumServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithTopSellingAlbums()
        {
            // Arrange
            var albums = new List<AlbumDto>
            {
                new AlbumDto { AlbumId = 1, Title = "Album 1", Price = 9.99m },
                new AlbumDto { AlbumId = 2, Title = "Album 2", Price = 14.99m }
            };
            _albumServiceMock.Setup(s => s.GetTopSellingAlbumsAsync(5)).ReturnsAsync(albums);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<AlbumDto>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Index_EmptyAlbums_ReturnsViewWithEmptyList()
        {
            // Arrange
            _albumServiceMock.Setup(s => s.GetTopSellingAlbumsAsync(5)).ReturnsAsync(new List<AlbumDto>());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<AlbumDto>>(viewResult.Model);
            Assert.Empty(model);
        }

        [Fact]
        public void Error_ReturnsView()
        {
            // Act
            var result = _controller.Error();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Index_CallsServiceWithCount5()
        {
            // Arrange
            _albumServiceMock.Setup(s => s.GetTopSellingAlbumsAsync(5)).ReturnsAsync(new List<AlbumDto>());

            // Act
            await _controller.Index();

            // Assert
            _albumServiceMock.Verify(s => s.GetTopSellingAlbumsAsync(5), Times.Once);
        }
    }
}
