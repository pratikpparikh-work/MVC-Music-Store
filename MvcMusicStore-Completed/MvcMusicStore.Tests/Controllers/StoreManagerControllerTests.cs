using System.Collections.Generic;
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
    /// Unit tests for StoreManagerController.
    /// </summary>
    public class StoreManagerControllerTests
    {
        private readonly Mock<IAlbumService> _mockAlbumService;
        private readonly Mock<IGenreService> _mockGenreService;
        private readonly Mock<IArtistService> _mockArtistService;
        private readonly StoreManagerController _controller;

        public StoreManagerControllerTests()
        {
            _mockAlbumService = new Mock<IAlbumService>();
            _mockGenreService = new Mock<IGenreService>();
            _mockArtistService = new Mock<IArtistService>();
            _controller = new StoreManagerController(
                _mockAlbumService.Object,
                _mockGenreService.Object,
                _mockArtistService.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithAllAlbums()
        {
            // Arrange
            var albums = new List<Album>
            {
                new Album { AlbumId = 1, Title = "Album 1" },
                new Album { AlbumId = 2, Title = "Album 2" }
            };
            _mockAlbumService.Setup(s => s.GetAllAlbumsAsync())
                .ReturnsAsync(albums);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsViewWithAlbum()
        {
            // Arrange
            var album = new Album { AlbumId = 1, Title = "Test Album" };
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
        public async Task Create_Get_ReturnsView()
        {
            // Arrange
            _mockGenreService.Setup(s => s.GetAllGenresAsync())
                .ReturnsAsync(new List<Genre>());
            _mockArtistService.Setup(s => s.GetAllArtistsAsync())
                .ReturnsAsync(new List<Artist>());

            // Act
            var result = await _controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            var album = new Album { Title = "New Album", Price = 9.99m, GenreId = 1, ArtistId = 1 };
            _mockAlbumService.Setup(s => s.CreateAlbumAsync(album))
                .ReturnsAsync(album);

            // Act
            var result = await _controller.Create(album);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_Post_WithInvalidModel_ReturnsView()
        {
            // Arrange
            var album = new Album { Title = "", Price = 0m };
            _controller.ModelState.AddModelError("Title", "Title is required");
            _mockGenreService.Setup(s => s.GetAllGenresAsync())
                .ReturnsAsync(new List<Genre>());
            _mockArtistService.Setup(s => s.GetAllArtistsAsync())
                .ReturnsAsync(new List<Artist>());

            // Act
            var result = await _controller.Create(album);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Edit_Get_WithValidId_ReturnsViewWithAlbum()
        {
            // Arrange
            var album = new Album { AlbumId = 1, Title = "Edit Album", GenreId = 1, ArtistId = 1 };
            _mockAlbumService.Setup(s => s.GetAlbumByIdAsync(1))
                .ReturnsAsync(album);
            _mockGenreService.Setup(s => s.GetAllGenresAsync())
                .ReturnsAsync(new List<Genre>());
            _mockArtistService.Setup(s => s.GetAllArtistsAsync())
                .ReturnsAsync(new List<Artist>());

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Album>(viewResult.Model);
            Assert.Equal("Edit Album", model.Title);
        }

        [Fact]
        public async Task Edit_Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockAlbumService.Setup(s => s.GetAlbumByIdAsync(999))
                .ReturnsAsync((Album?)null);

            // Act
            var result = await _controller.Edit(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            var album = new Album { AlbumId = 1, Title = "Updated Album", Price = 9.99m };
            _mockAlbumService.Setup(s => s.UpdateAlbumAsync(album))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Edit(album);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Edit_Post_WithInvalidModel_ReturnsView()
        {
            // Arrange
            var album = new Album { AlbumId = 1, Title = "" };
            _controller.ModelState.AddModelError("Title", "Title is required");
            _mockGenreService.Setup(s => s.GetAllGenresAsync())
                .ReturnsAsync(new List<Genre>());
            _mockArtistService.Setup(s => s.GetAllArtistsAsync())
                .ReturnsAsync(new List<Artist>());

            // Act
            var result = await _controller.Edit(album);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Delete_Get_WithValidId_ReturnsViewWithAlbum()
        {
            // Arrange
            var album = new Album { AlbumId = 1, Title = "Delete Album" };
            _mockAlbumService.Setup(s => s.GetAlbumByIdAsync(1))
                .ReturnsAsync(album);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Album>(viewResult.Model);
            Assert.Equal("Delete Album", model.Title);
        }

        [Fact]
        public async Task Delete_Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockAlbumService.Setup(s => s.GetAlbumByIdAsync(999))
                .ReturnsAsync((Album?)null);

            // Act
            var result = await _controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_CallsDeleteAndRedirectsToIndex()
        {
            // Arrange
            _mockAlbumService.Setup(s => s.DeleteAlbumAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            _mockAlbumService.Verify(s => s.DeleteAlbumAsync(1), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }
    }
}
