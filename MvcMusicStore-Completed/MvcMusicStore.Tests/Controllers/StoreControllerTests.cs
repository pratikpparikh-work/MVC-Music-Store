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
    /// Unit tests for StoreController.
    /// </summary>
    public class StoreControllerTests
    {
        private readonly Mock<IGenreService> _mockGenreService;
        private readonly Mock<IAlbumService> _mockAlbumService;
        private readonly StoreController _controller;

        public StoreControllerTests()
        {
            _mockGenreService = new Mock<IGenreService>();
            _mockAlbumService = new Mock<IAlbumService>();
            _controller = new StoreController(_mockGenreService.Object, _mockAlbumService.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithAllGenres()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new Genre { GenreId = 1, Name = "Rock" },
                new Genre { GenreId = 2, Name = "Jazz" }
            };
            _mockGenreService.Setup(s => s.GetAllGenresAsync())
                .ReturnsAsync(genres);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Genre>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Index_CallsGetAllGenresOnce()
        {
            // Arrange
            _mockGenreService.Setup(s => s.GetAllGenresAsync())
                .ReturnsAsync(new List<Genre>());

            // Act
            await _controller.Index();

            // Assert
            _mockGenreService.Verify(s => s.GetAllGenresAsync(), Times.Once);
        }

        [Fact]
        public async Task Browse_WithValidGenre_ReturnsViewWithGenre()
        {
            // Arrange
            var genre = new Genre { GenreId = 1, Name = "Rock", Description = "Rock music" };
            _mockGenreService.Setup(s => s.GetGenreByNameAsync("Rock"))
                .ReturnsAsync(genre);

            // Act
            var result = await _controller.Browse("Rock");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Genre>(viewResult.Model);
            Assert.Equal("Rock", model.Name);
        }

        [Fact]
        public async Task Browse_WithInvalidGenre_ReturnsNotFound()
        {
            // Arrange
            _mockGenreService.Setup(s => s.GetGenreByNameAsync("NonExistent"))
                .ReturnsAsync((Genre?)null);

            // Act
            var result = await _controller.Browse("NonExistent");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsViewWithAlbum()
        {
            // Arrange
            var album = new Album { AlbumId = 1, Title = "Test Album", Price = 9.99m };
            _mockAlbumService.Setup(s => s.GetAlbumByIdAsync(1))
                .ReturnsAsync(album);

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Album>(viewResult.Model);
            Assert.Equal("Test Album", model.Title);
        }

        [Fact]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockAlbumService.Setup(s => s.GetAlbumByIdAsync(999))
                .ReturnsAsync((Album?)null);

            // Act
            var result = await _controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Browse_CallsGetGenreByNameWithCorrectName()
        {
            // Arrange
            var genre = new Genre { GenreId = 1, Name = "Jazz" };
            _mockGenreService.Setup(s => s.GetGenreByNameAsync("Jazz"))
                .ReturnsAsync(genre);

            // Act
            await _controller.Browse("Jazz");

            // Assert
            _mockGenreService.Verify(s => s.GetGenreByNameAsync("Jazz"), Times.Once);
        }

        [Fact]
        public async Task Details_CallsGetAlbumByIdWithCorrectId()
        {
            // Arrange
            var album = new Album { AlbumId = 5, Title = "Album 5" };
            _mockAlbumService.Setup(s => s.GetAlbumByIdAsync(5))
                .ReturnsAsync(album);

            // Act
            await _controller.Details(5);

            // Assert
            _mockAlbumService.Verify(s => s.GetAlbumByIdAsync(5), Times.Once);
        }
    }
}
