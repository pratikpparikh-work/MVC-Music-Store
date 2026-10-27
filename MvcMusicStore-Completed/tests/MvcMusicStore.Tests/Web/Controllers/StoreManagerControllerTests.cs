using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using MvcMusicStore.Application.DTOs;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Web.Controllers;
using Xunit;

namespace MvcMusicStore.Tests.Web.Controllers
{
    public class StoreManagerControllerTests
    {
        private readonly Mock<IAlbumService> _albumServiceMock;
        private readonly Mock<IGenreService> _genreServiceMock;
        private readonly Mock<IArtistService> _artistServiceMock;
        private readonly Mock<ILogger<StoreManagerController>> _loggerMock;
        private readonly StoreManagerController _controller;

        public StoreManagerControllerTests()
        {
            _albumServiceMock = new Mock<IAlbumService>();
            _genreServiceMock = new Mock<IGenreService>();
            _artistServiceMock = new Mock<IArtistService>();
            _loggerMock = new Mock<ILogger<StoreManagerController>>();

            _controller = new StoreManagerController(
                _albumServiceMock.Object,
                _genreServiceMock.Object,
                _artistServiceMock.Object,
                _loggerMock.Object);

            // Setup ViewBag/TempData
            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>());
        }

        private void SetupDropDowns()
        {
            _genreServiceMock.Setup(s => s.GetAllGenresAsync()).ReturnsAsync(new List<GenreDto>
            {
                new GenreDto { GenreId = 1, Name = "Rock" }
            });
            _artistServiceMock.Setup(s => s.GetAllArtistsAsync()).ReturnsAsync(new List<ArtistDto>
            {
                new ArtistDto { ArtistId = 1, Name = "Artist 1" }
            });
        }

        [Fact]
        public async Task Index_ReturnsViewWithAlbums()
        {
            // Arrange
            var albums = new List<AlbumDto>
            {
                new AlbumDto { AlbumId = 1, Title = "Album 1" },
                new AlbumDto { AlbumId = 2, Title = "Album 2" }
            };
            _albumServiceMock.Setup(s => s.GetAllAlbumsAsync()).ReturnsAsync(albums);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<AlbumDto>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Details_ExistingAlbum_ReturnsViewWithAlbum()
        {
            // Arrange
            var album = new AlbumDto { AlbumId = 1, Title = "Test Album" };
            _albumServiceMock.Setup(s => s.GetAlbumByIdAsync(1)).ReturnsAsync(album);

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AlbumDto>(viewResult.Model);
            Assert.Equal(1, model.AlbumId);
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
        public async Task Create_Get_ReturnsView()
        {
            // Arrange
            SetupDropDowns();

            // Act
            var result = await _controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            // Arrange
            SetupDropDowns();
            var dto = new AlbumDto { Title = "New Album", Price = 9.99m, GenreId = 1, ArtistId = 1 };
            _albumServiceMock.Setup(s => s.CreateAlbumAsync(dto)).ReturnsAsync(dto);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_Post_InvalidModel_ReturnsView()
        {
            // Arrange
            SetupDropDowns();
            _controller.ModelState.AddModelError("Title", "Required");
            var dto = new AlbumDto();

            // Act
            var result = await _controller.Create(dto);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Edit_Get_ExistingAlbum_ReturnsView()
        {
            // Arrange
            SetupDropDowns();
            var album = new AlbumDto { AlbumId = 1, Title = "Test Album", GenreId = 1, ArtistId = 1 };
            _albumServiceMock.Setup(s => s.GetAlbumByIdAsync(1)).ReturnsAsync(album);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<AlbumDto>(viewResult.Model);
        }

        [Fact]
        public async Task Edit_Get_NonExistingAlbum_ReturnsNotFound()
        {
            // Arrange
            _albumServiceMock.Setup(s => s.GetAlbumByIdAsync(999)).ReturnsAsync((AlbumDto?)null);

            // Act
            var result = await _controller.Edit(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var dto = new AlbumDto { AlbumId = 1, Title = "Updated Album", Price = 9.99m, GenreId = 1, ArtistId = 1 };
            _albumServiceMock.Setup(s => s.UpdateAlbumAsync(dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Edit(dto);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Edit_Post_InvalidModel_ReturnsView()
        {
            // Arrange
            SetupDropDowns();
            _controller.ModelState.AddModelError("Title", "Required");
            var dto = new AlbumDto();

            // Act
            var result = await _controller.Edit(dto);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Delete_Get_ExistingAlbum_ReturnsView()
        {
            // Arrange
            var album = new AlbumDto { AlbumId = 1, Title = "Test Album" };
            _albumServiceMock.Setup(s => s.GetAlbumByIdAsync(1)).ReturnsAsync(album);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<AlbumDto>(viewResult.Model);
        }

        [Fact]
        public async Task Delete_Get_NonExistingAlbum_ReturnsNotFound()
        {
            // Arrange
            _albumServiceMock.Setup(s => s.GetAlbumByIdAsync(999)).ReturnsAsync((AlbumDto?)null);

            // Act
            var result = await _controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_Post_DeletesAndRedirects()
        {
            // Arrange
            _albumServiceMock.Setup(s => s.DeleteAlbumAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _albumServiceMock.Verify(s => s.DeleteAlbumAsync(1), Times.Once);
        }
    }
}
