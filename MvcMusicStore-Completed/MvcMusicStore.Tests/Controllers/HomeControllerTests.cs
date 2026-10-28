using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Web.Controllers;
using Xunit;

namespace MvcMusicStore.Tests.Controllers
{
    /// <summary>
    /// Unit tests for HomeController.
    /// </summary>
    public class HomeControllerTests
    {
        private readonly Mock<IAlbumService> _mockAlbumService;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockAlbumService = new Mock<IAlbumService>();
            _controller = new HomeController(_mockAlbumService.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithTopSellingAlbums()
        {
            // Arrange
            var topAlbums = new List<Album>
            {
                new Album { AlbumId = 1, Title = "Top Album 1" },
                new Album { AlbumId = 2, Title = "Top Album 2" },
                new Album { AlbumId = 3, Title = "Top Album 3" },
                new Album { AlbumId = 4, Title = "Top Album 4" },
                new Album { AlbumId = 5, Title = "Top Album 5" }
            };
            _mockAlbumService.Setup(s => s.GetTopSellingAlbumsAsync(5))
                .ReturnsAsync(topAlbums);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Album>>(viewResult.Model);
            Assert.Equal(5, model.Count());
        }

        [Fact]
        public async Task Index_CallsGetTopSellingAlbumsWithCount5()
        {
            // Arrange
            _mockAlbumService.Setup(s => s.GetTopSellingAlbumsAsync(5))
                .ReturnsAsync(new List<Album>());

            // Act
            await _controller.Index();

            // Assert
            _mockAlbumService.Verify(s => s.GetTopSellingAlbumsAsync(5), Times.Once);
        }

        [Fact]
        public async Task Index_WhenNoAlbums_ReturnsViewWithEmptyList()
        {
            // Arrange
            _mockAlbumService.Setup(s => s.GetTopSellingAlbumsAsync(5))
                .ReturnsAsync(new List<Album>());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Album>>(viewResult.Model);
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
        public async Task Index_ReturnsViewResult()
        {
            // Arrange
            _mockAlbumService.Setup(s => s.GetTopSellingAlbumsAsync(5))
                .ReturnsAsync(new List<Album>());

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
