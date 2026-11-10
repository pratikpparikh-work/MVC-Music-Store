using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using MvcMusicStore.Controllers;
using MvcMusicStore.Data;
using MvcMusicStore.Models;
using MvcMusicStore.Tests.Helpers;

namespace MvcMusicStore.Tests.Controllers
{
    public class StoreManagerControllerTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly TestApplicationDbContext _context;
        private readonly StoreManagerController _controller;

        public StoreManagerControllerTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;
            _context = new TestApplicationDbContext(options);
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
            var result = await _controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.List<Album>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WhenAlbumExists()
        {
            var album = await _context.Albums.FirstAsync();
            var result = await _controller.Details(album.AlbumId);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Album>(viewResult.Model);
            Assert.Equal(album.AlbumId, model.AlbumId);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenAlbumDoesNotExist()
        {
            var result = await _controller.Details(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_Get_ReturnsViewResult()
        {
            var result = _controller.Create();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            var genre = await _context.Genres.FirstAsync();
            var artist = await _context.Artists.FirstAsync();
            var album = new Album { Title = "New Album", Price = 9.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId };
            var result = await _controller.Create(album);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_Post_ValidModel_AddsAlbumToDatabase()
        {
            var genre = await _context.Genres.FirstAsync();
            var artist = await _context.Artists.FirstAsync();
            var album = new Album { Title = "New Album", Price = 9.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId };
            var initialCount = await _context.Albums.CountAsync();
            await _controller.Create(album);
            var finalCount = await _context.Albums.CountAsync();
            Assert.Equal(initialCount + 1, finalCount);
        }

        [Fact]
        public async Task Create_Post_InvalidModel_ReturnsViewResult()
        {
            _controller.ModelState.AddModelError("Title", "Title is required");
            var album = new Album();
            var result = await _controller.Create(album);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Edit_Get_ReturnsViewResult_WhenAlbumExists()
        {
            var album = await _context.Albums.FirstAsync();
            var result = await _controller.Edit(album.AlbumId);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Album>(viewResult.Model);
            Assert.Equal(album.AlbumId, model.AlbumId);
        }

        [Fact]
        public async Task Edit_Get_ReturnsNotFound_WhenAlbumDoesNotExist()
        {
            var result = await _controller.Edit(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ValidModel_RedirectsToIndex()
        {
            // Get album ID without tracking
            var albumId = (await _context.Albums.AsNoTracking().FirstAsync()).AlbumId;
            var genreId = (await _context.Genres.AsNoTracking().FirstAsync()).GenreId;
            var artistId = (await _context.Artists.AsNoTracking().FirstAsync()).ArtistId;
            // Detach all tracked entities to avoid conflicts
            foreach (var entry in _context.ChangeTracker.Entries().ToList())
            {
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
            // Create a detached album with the same ID
            var album = new Album { AlbumId = albumId, Title = "Updated Title", Price = 9.99m, GenreId = genreId, ArtistId = artistId };
            var result = await _controller.Edit(album);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Edit_Post_InvalidModel_ReturnsViewResult()
        {
            _controller.ModelState.AddModelError("Title", "Title is required");
            var album = new Album();
            var result = await _controller.Edit(album);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Delete_Get_ReturnsViewResult_WhenAlbumExists()
        {
            var album = await _context.Albums.FirstAsync();
            var result = await _controller.Delete(album.AlbumId);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Album>(viewResult.Model);
            Assert.Equal(album.AlbumId, model.AlbumId);
        }

        [Fact]
        public async Task Delete_Get_ReturnsNotFound_WhenAlbumDoesNotExist()
        {
            var result = await _controller.Delete(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesAlbum_AndRedirectsToIndex()
        {
            var album = await _context.Albums.FirstAsync();
            var albumId = album.AlbumId;
            var initialCount = await _context.Albums.CountAsync();
            var result = await _controller.DeleteConfirmed(albumId);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            var finalCount = await _context.Albums.CountAsync();
            Assert.Equal(initialCount - 1, finalCount);
        }

        [Fact]
        public async Task DeleteConfirmed_WithNonExistentId_RedirectsToIndex()
        {
            var result = await _controller.DeleteConfirmed(9999);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }
    }
}
