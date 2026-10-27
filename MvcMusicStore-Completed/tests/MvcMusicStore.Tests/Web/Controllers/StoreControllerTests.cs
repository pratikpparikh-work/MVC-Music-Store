using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MvcMusicStore.Application.DTOs;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Web.Controllers;
using Xunit;

namespace MvcMusicStore.Tests.Web.Controllers
{
    public class StoreControllerTests
    {
        private readonly Mock<IGenreService> _genreServiceMock;
        private readonly Mock<IAlbumService> _albumServiceMock;
        private readonly Mock<ILogger<StoreController>> _loggerMock;
        private readonly StoreController _controller;

        public StoreControllerTests()
        {
            _genreServiceMock = new Mock<IGenreService>();
            _albumServiceMock = new Mock<IAlbumService>();
            _loggerMock = new Mock<ILogger<StoreController>>();
            _controller = new StoreController(_genreServiceMock.Object, _albumServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithGenres()
        {
            // Arrange
            var genres = new List<GenreDto>
            {
                new GenreDto { GenreId = 1, Name = "Rock" },
                new GenreDto { GenreId = 2, Name = "Jazz" }
            };
            _genreServiceMock.Setup(s => s.GetAllGenresAsync()).ReturnsAsync(genres);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<GenreDto>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Browse_ExistingGenre_ReturnsViewWithGenre()
        {
            // Arrange
            var genre = new GenreDto { GenreId = 1, Name = "Rock", Description = "Rock music" };
            _genreServiceMock.Setup(s => s.GetGenreWithAlbumsAsync("Rock")).ReturnsAsync(genre);

            // Act
            var result = await _controller.Browse("Rock");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<GenreDto>(viewResult.Model);
            Assert.Equal("Rock", model.Name);
        }

        [Fact]
        public async Task Browse_NonExistingGenre_ReturnsNotFound()
        {
            // Arrange
            _genreServiceMock.Setup(s => s.GetGenreWithAlbumsAsync("Unknown")).ReturnsAsync((GenreDto?)null);

            // Act
            var result = await _controller.Browse("Unknown");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ExistingAlbum_ReturnsViewWithAlbum()
        {
            // Arrange
            var album = new AlbumDto { AlbumId = 1, Title = "Test Album", Price = 9.99m };
            _albumServiceMock.Setup(s => s.GetAlbumByIdAsync(1)).ReturnsAsync(album);

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AlbumDto>(viewResult.Model);
            Assert.Equal(1, model.AlbumId);
            Assert.Equal("Test Album", model.Title);
        }

        [Fact]
        public async Task Details_NonExistingAlbum_ReturnsNotFound()
        {
            // Arrange
            _albumServiceMock.Setup(s => s.GetAlbumByIdAsync(999)).ReturnsAsync((AlbumDto?)null);

            // Act
            var result = await _controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Index_EmptyGenres_ReturnsViewWithEmptyList()
        {
            // Arrange
            _genreServiceMock.Setup(s => s.GetAllGenresAsync()).ReturnsAsync(new List<GenreDto>());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<GenreDto>>(viewResult.Model);
            Assert.Empty(model);
        }
    }
}
