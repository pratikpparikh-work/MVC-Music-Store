using System;
using System.Collections.Generic;
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
    public class HomeControllerTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ApplicationDbContext _context;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;
            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();
            _controller = new HomeController(_context);
        }

        public void Dispose()
        {
            _controller.Dispose();
            _context.Dispose();
            _connection.Dispose();
        }

        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            // Arrange
            SeedAlbums();

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
        }

        [Fact]
        public async Task Index_ReturnsTopFiveAlbums_WhenMoreThanFiveExist()
        {
            // Arrange
            SeedAlbums(10);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Album>>(viewResult.Model);
            Assert.True(model.Count <= 5);
        }

        [Fact]
        public async Task Index_ReturnsEmptyList_WhenNoAlbumsExist()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Album>>(viewResult.Model);
            Assert.Empty(model);
        }

        [Fact]
        public void Error_ReturnsViewResult()
        {
            // Act
            var result = _controller.Error();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Index_ReturnsAllAlbums_WhenFiveOrFewerExist()
        {
            // Arrange
            SeedAlbums(3);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Album>>(viewResult.Model);
            Assert.Equal(3, model.Count);
        }

        private void SeedAlbums(int count = 5)
        {
            var genre = new Genre { Name = "Rock" };
            var artist = new Artist { Name = "Test Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            _context.SaveChanges();

            for (int i = 1; i <= count; i++)
            {
                _context.Albums.Add(new Album
                {
                    Title = $"Album {i}",
                    Price = 8.99m,
                    GenreId = genre.GenreId,
                    ArtistId = artist.ArtistId
                });
            }
            _context.SaveChanges();
        }
    }
}
