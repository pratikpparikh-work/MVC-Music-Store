using System;
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
    public class StoreControllerTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly TestApplicationDbContext _context;
        private readonly StoreController _controller;

        public StoreControllerTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;
            _context = new TestApplicationDbContext(options);
            _context.Database.EnsureCreated();
            _controller = new StoreController(_context);
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
            var genre1 = new Genre { Name = "Rock", Description = "Rock music" };
            var genre2 = new Genre { Name = "Jazz", Description = "Jazz music" };
            var artist = new Artist { Name = "Test Artist" };
            _context.Genres.AddRange(genre1, genre2);
            _context.Artists.Add(artist);
            _context.SaveChanges();
            _context.Albums.AddRange(
                new Album { Title = "Rock Album 1", Price = 8.99m, GenreId = genre1.GenreId, ArtistId = artist.ArtistId },
                new Album { Title = "Rock Album 2", Price = 9.99m, GenreId = genre1.GenreId, ArtistId = artist.ArtistId },
                new Album { Title = "Jazz Album 1", Price = 7.99m, GenreId = genre2.GenreId, ArtistId = artist.ArtistId }
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithGenreList()
        {
            var result = await _controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.List<Genre>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Browse_ReturnsViewResult_WhenGenreExists()
        {
            var result = await _controller.Browse("Rock");
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Genre>(viewResult.Model);
            Assert.Equal("Rock", model.Name);
        }

        [Fact]
        public async Task Browse_ReturnsNotFound_WhenGenreDoesNotExist()
        {
            var result = await _controller.Browse("NonExistentGenre");
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Browse_ReturnsGenreWithAlbums_WhenGenreExists()
        {
            var result = await _controller.Browse("Rock");
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Genre>(viewResult.Model);
            Assert.Equal(2, model.Albums.Count);
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
        public async Task Details_ReturnsCorrectAlbum_ByTitle()
        {
            var album = await _context.Albums.FirstAsync(a => a.Title == "Rock Album 1");
            var result = await _controller.Details(album.AlbumId);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Album>(viewResult.Model);
            Assert.Equal("Rock Album 1", model.Title);
        }

        [Fact]
        public async Task Browse_WithEmptyGenreName_ReturnsNotFound()
        {
            var result = await _controller.Browse("");
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Index_ReturnsAllGenres()
        {
            var result = await _controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.List<Genre>>(viewResult.Model);
            Assert.NotEmpty(model);
        }
    }
}
