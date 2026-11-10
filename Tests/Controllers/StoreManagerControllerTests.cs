using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using MvcMusicStore.Controllers;
using MvcMusicStore.Data;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Controllers
{
    public class StoreManagerControllerTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ApplicationDbContext _context;
        private readonly StoreManagerController _controller;

        public StoreManagerControllerTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;
            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();
            _controller = new StoreManagerController(_context);
            SeedData();
        }

        public void Dispose()
        {
            _controller.Dispose();
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

            _context.Albums.AddRange(
                new Album { Title = "Album 1", Price = 8.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId },
                new Album { Title = "Album 2", Price = 9.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId }
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithAlbumList()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.List<Album>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WhenAlbumExists()
        {
            // Arrange
            var album = await _context.Albums.FirstAsync();

            // Act
            var result = await _controller.Details(album.AlbumId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Album>(viewResult.Model);
            Assert.Equal(album.AlbumId, model.AlbumId);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenAlbumDoesNotExist()
        {
            // Act
            var result = await _controller.Details(9999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var genre = await _context.Genres.FirstAsync();
            var artist = await _context.Artists.FirstAsync();
            var album = new Album
            {
                Title = "New Album",
                Price = 9.99m,
                GenreId = genre.GenreId,
                ArtistId = artist.ArtistId
            };

            // Act
            var result = await _controller.Create(album);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_Post_ValidModel_AddsAlbumToDatabase()
        {
            // Arrange
            var genre = await _context.Genres.FirstAsync();
            var artist = await _context.Artists.FirstAsync();
            var album = new Album
            {
                Title = "New Album",
                Price = 9.99m,
                GenreId = genre.GenreId,
                ArtistId = artist.ArtistId
            };
            var initialCount = await _context.Albums.CountAsync();

            // Act
            await _controller.Create(album);

            // Assert
            var finalCount = await _context.Albums.CountAsync();
            Assert.Equal(initialCount + 1, finalCount);
        }

        [Fact]
        public async Task Create_Post_InvalidModel_ReturnsViewResult()
        {
            // Arrange
            _controller.ModelState.AddModelError("Title", "Title is required");
            var album = new Album();

            // Act
            var result = await _controller.Create(album);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Edit_Get_ReturnsViewResult_WhenAlbumExists()
        {
            // Arrange
            var album = await _context.Albums.FirstAsync();

            // Act
            var result = await _controller.Edit(album.AlbumId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Album>(viewResult.Model);
            Assert.Equal(album.AlbumId, model.AlbumId);
        }

        [Fact]
        public async Task Edit_Get_ReturnsNotFound_WhenAlbumDoesNotExist()
        {
            // Act
            var result = await _controller.Edit(9999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var album = await _context.Albums.AsNoTracking().FirstAsync();
            album.Title = "Updated Title";

            // Act
            var result = await _controller.Edit(album);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Edit_Post_InvalidModel_ReturnsViewResult()
        {
            // Arrange
            _controller.ModelState.AddModelError("Title", "Title is required");
            var album = new Album();

            // Act
            var result = await _controller.Edit(album);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Delete_Get_ReturnsViewResult_WhenAlbumExists()
        {
            // Arrange
            var album = await _context.Albums.FirstAsync();

            // Act
            var result = await _controller.Delete(album.AlbumId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Album>(viewResult.Model);
            Assert.Equal(album.AlbumId, model.AlbumId);
        }

        [Fact]
        public async Task Delete_Get_ReturnsNotFound_WhenAlbumDoesNotExist()
        {
            // Act
            var result = await _controller.Delete(9999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesAlbum_AndRedirectsToIndex()
        {
            // Arrange
            var album = await _context.Albums.FirstAsync();
            var albumId = album.AlbumId;
            var initialCount = await _context.Albums.CountAsync();

            // Act
            var result = await _controller.DeleteConfirmed(albumId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            var finalCount = await _context.Albums.CountAsync();
            Assert.Equal(initialCount - 1, finalCount);
        }

        [Fact]
        public async Task DeleteConfirmed_WithNonExistentId_RedirectsToIndex()
        {
            // Act
            var result = await _controller.DeleteConfirmed(9999);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }
    }
}
