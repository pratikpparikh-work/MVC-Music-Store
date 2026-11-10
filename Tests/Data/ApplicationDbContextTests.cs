using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using MvcMusicStore.Data;
using MvcMusicStore.Models;
using MvcMusicStore.Tests.Helpers;

namespace MvcMusicStore.Tests.Data
{
    public class ApplicationDbContextTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly TestApplicationDbContext _context;

        public ApplicationDbContextTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;
            _context = new TestApplicationDbContext(options);
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }

        [Fact]
        public void ApplicationDbContext_CanBeCreated()
        {
            Assert.NotNull(_context);
        }

        [Fact]
        public void Albums_DbSet_IsNotNull()
        {
            Assert.NotNull(_context.Albums);
        }

        [Fact]
        public void Genres_DbSet_IsNotNull()
        {
            Assert.NotNull(_context.Genres);
        }

        [Fact]
        public void Artists_DbSet_IsNotNull()
        {
            Assert.NotNull(_context.Artists);
        }

        [Fact]
        public void Carts_DbSet_IsNotNull()
        {
            Assert.NotNull(_context.Carts);
        }

        [Fact]
        public void Orders_DbSet_IsNotNull()
        {
            Assert.NotNull(_context.Orders);
        }

        [Fact]
        public void OrderDetails_DbSet_IsNotNull()
        {
            Assert.NotNull(_context.OrderDetails);
        }

        [Fact]
        public async Task CanAddAndRetrieveGenre()
        {
            var genre = new Genre { Name = "Rock", Description = "Rock music" };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
            var retrieved = await _context.Genres.FindAsync(genre.GenreId);
            Assert.NotNull(retrieved);
            Assert.Equal("Rock", retrieved.Name);
        }

        [Fact]
        public async Task CanAddAndRetrieveArtist()
        {
            var artist = new Artist { Name = "Metallica" };
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();
            var retrieved = await _context.Artists.FindAsync(artist.ArtistId);
            Assert.NotNull(retrieved);
            Assert.Equal("Metallica", retrieved.Name);
        }

        [Fact]
        public async Task CanAddAndRetrieveAlbum()
        {
            var genre = new Genre { Name = "Metal" };
            var artist = new Artist { Name = "Metallica" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();
            var album = new Album { Title = "Master Of Puppets", Price = 8.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();
            var retrieved = await _context.Albums.FindAsync(album.AlbumId);
            Assert.NotNull(retrieved);
            Assert.Equal("Master Of Puppets", retrieved.Title);
            Assert.Equal(8.99m, retrieved.Price);
        }

        [Fact]
        public async Task CanAddAndRetrieveCart()
        {
            var genre = new Genre { Name = "Rock" };
            var artist = new Artist { Name = "Test Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();
            var album = new Album { Title = "Test", Price = 9.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();
            var cart = new Cart { CartId = "test-cart", AlbumId = album.AlbumId, Count = 1, DateCreated = DateTime.Now };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            var retrieved = await _context.Carts.FindAsync(cart.RecordId);
            Assert.NotNull(retrieved);
            Assert.Equal("test-cart", retrieved.CartId);
            Assert.Equal(1, retrieved.Count);
        }

        [Fact]
        public async Task CanAddAndRetrieveOrder()
        {
            var order = new Order
            {
                Username = "testuser", FirstName = "John", LastName = "Doe",
                Address = "123 Main St", City = "Seattle", State = "WA",
                PostalCode = "98101", Country = "USA", Phone = "555-1234",
                Email = "john@example.com", Total = 49.99m, OrderDate = DateTime.Now
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            var retrieved = await _context.Orders.FindAsync(order.OrderId);
            Assert.NotNull(retrieved);
            Assert.Equal("testuser", retrieved.Username);
            Assert.Equal(49.99m, retrieved.Total);
        }

        [Fact]
        public async Task CanAddAndRetrieveOrderDetail()
        {
            var genre = new Genre { Name = "Rock" };
            var artist = new Artist { Name = "Test Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();
            var album = new Album { Title = "Test", Price = 9.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId };
            _context.Albums.Add(album);
            var order = new Order
            {
                Username = "testuser", FirstName = "John", LastName = "Doe",
                Address = "123 Main St", City = "Seattle", State = "WA",
                PostalCode = "98101", Country = "USA", Phone = "555-1234",
                Email = "john@example.com", OrderDate = DateTime.Now
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            var detail = new OrderDetail { OrderId = order.OrderId, AlbumId = album.AlbumId, Quantity = 2, UnitPrice = 9.99m };
            _context.OrderDetails.Add(detail);
            await _context.SaveChangesAsync();
            var retrieved = await _context.OrderDetails.FindAsync(detail.OrderDetailId);
            Assert.NotNull(retrieved);
            Assert.Equal(2, retrieved.Quantity);
            Assert.Equal(9.99m, retrieved.UnitPrice);
        }

        [Fact]
        public async Task CanDeleteGenre()
        {
            var genre = new Genre { Name = "ToDelete" };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
            var retrieved = await _context.Genres.FindAsync(genre.GenreId);
            Assert.Null(retrieved);
        }

        [Fact]
        public async Task CanUpdateAlbum()
        {
            var genre = new Genre { Name = "Rock" };
            var artist = new Artist { Name = "Test Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();
            var album = new Album { Title = "Original Title", Price = 8.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();
            album.Title = "Updated Title";
            album.Price = 12.99m;
            await _context.SaveChangesAsync();
            var retrieved = await _context.Albums.FindAsync(album.AlbumId);
            Assert.NotNull(retrieved);
            Assert.Equal("Updated Title", retrieved.Title);
            Assert.Equal(12.99m, retrieved.Price);
        }

        [Fact]
        public async Task CanCountAlbums()
        {
            var genre = new Genre { Name = "Rock" };
            var artist = new Artist { Name = "Test Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();
            _context.Albums.AddRange(
                new Album { Title = "Album 1", Price = 8.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId },
                new Album { Title = "Album 2", Price = 9.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId }
            );
            await _context.SaveChangesAsync();
            var count = await _context.Albums.CountAsync();
            Assert.Equal(2, count);
        }
    }
}
