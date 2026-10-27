using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Infrastructure.Data;
using MvcMusicStore.Infrastructure.Repositories;
using Xunit;

namespace MvcMusicStore.Tests.Infrastructure
{
    public class RepositoryTests : IDisposable
    {
        private readonly MusicStoreDbContext _context;

        public RepositoryTests()
        {
            var options = new DbContextOptionsBuilder<MusicStoreDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new MusicStoreDbContext(options);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        // ─── Generic Repository ──────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ExistingEntity_ReturnsEntity()
        {
            // Arrange
            var genre = new Genre { Name = "Rock", Description = "Rock music" };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
            var repo = new GenreRepository(_context);

            // Act
            var result = await repo.GetByIdAsync(genre.GenreId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Rock", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingEntity_ReturnsNull()
        {
            // Arrange
            var repo = new GenreRepository(_context);

            // Act
            var result = await repo.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllEntities()
        {
            // Arrange
            _context.Genres.AddRange(
                new Genre { Name = "Rock", Description = "Rock" },
                new Genre { Name = "Jazz", Description = "Jazz" }
            );
            await _context.SaveChangesAsync();
            var repo = new GenreRepository(_context);

            // Act
            var result = await repo.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AddAsync_AddsEntity()
        {
            // Arrange
            var repo = new GenreRepository(_context);
            var genre = new Genre { Name = "Pop", Description = "Pop music" };

            // Act
            await repo.AddAsync(genre);
            await repo.SaveChangesAsync();

            // Assert
            Assert.Equal(1, _context.Genres.Count());
        }

        [Fact]
        public async Task Update_UpdatesEntity()
        {
            // Arrange
            var genre = new Genre { Name = "Rock", Description = "Rock music" };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
            var repo = new GenreRepository(_context);

            // Act
            genre.Name = "Updated Rock";
            repo.Update(genre);
            await repo.SaveChangesAsync();

            // Assert
            var updated = await _context.Genres.FindAsync(genre.GenreId);
            Assert.Equal("Updated Rock", updated!.Name);
        }

        [Fact]
        public async Task Remove_RemovesEntity()
        {
            // Arrange
            var genre = new Genre { Name = "Rock", Description = "Rock music" };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
            var repo = new GenreRepository(_context);

            // Act
            repo.Remove(genre);
            await repo.SaveChangesAsync();

            // Assert
            Assert.Equal(0, _context.Genres.Count());
        }

        // ─── AlbumRepository ─────────────────────────────────────────────────────────

        [Fact]
        public async Task AlbumRepository_GetAlbumsWithDetailsAsync_ReturnsAlbumsWithDetails()
        {
            // Arrange
            var genre = new Genre { Name = "Rock", Description = "Rock" };
            var artist = new Artist { Name = "Test Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();

            var album = new Album { Title = "Test Album", Price = 9.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var repo = new AlbumRepository(_context);

            // Act
            var result = await repo.GetAlbumsWithDetailsAsync();

            // Assert
            var list = result.ToList();
            Assert.Single(list);
            Assert.Equal("Test Album", list[0].Title);
            Assert.NotNull(list[0].Genre);
            Assert.NotNull(list[0].Artist);
        }

        [Fact]
        public async Task AlbumRepository_GetAlbumWithDetailsAsync_ExistingId_ReturnsAlbum()
        {
            // Arrange
            var genre = new Genre { Name = "Rock", Description = "Rock" };
            var artist = new Artist { Name = "Test Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();

            var album = new Album { Title = "Test Album", Price = 9.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var repo = new AlbumRepository(_context);

            // Act
            var result = await repo.GetAlbumWithDetailsAsync(album.AlbumId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Album", result.Title);
        }

        [Fact]
        public async Task AlbumRepository_GetAlbumWithDetailsAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            var repo = new AlbumRepository(_context);

            // Act
            var result = await repo.GetAlbumWithDetailsAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AlbumRepository_GetTopSellingAlbumsAsync_ReturnsTopAlbums()
        {
            // Arrange
            var genre = new Genre { Name = "Rock", Description = "Rock" };
            var artist = new Artist { Name = "Test Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();

            _context.Albums.AddRange(
                new Album { Title = "Album 1", Price = 9.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId },
                new Album { Title = "Album 2", Price = 14.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId },
                new Album { Title = "Album 3", Price = 19.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId }
            );
            await _context.SaveChangesAsync();

            var repo = new AlbumRepository(_context);

            // Act
            var result = await repo.GetTopSellingAlbumsAsync(2);

            // Assert
            Assert.Equal(2, result.Count());
        }

        // ─── GenreRepository ─────────────────────────────────────────────────────────

        [Fact]
        public async Task GenreRepository_GetGenreByNameAsync_ExistingName_ReturnsGenre()
        {
            // Arrange
            _context.Genres.Add(new Genre { Name = "Rock", Description = "Rock music" });
            await _context.SaveChangesAsync();
            var repo = new GenreRepository(_context);

            // Act
            var result = await repo.GetGenreByNameAsync("Rock");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Rock", result.Name);
        }

        [Fact]
        public async Task GenreRepository_GetGenreByNameAsync_NonExistingName_ReturnsNull()
        {
            // Arrange
            var repo = new GenreRepository(_context);

            // Act
            var result = await repo.GetGenreByNameAsync("Unknown");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GenreRepository_GetGenreWithAlbumsAsync_ReturnsGenreWithAlbums()
        {
            // Arrange
            var genre = new Genre { Name = "Rock", Description = "Rock music" };
            var artist = new Artist { Name = "Test Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();

            _context.Albums.Add(new Album { Title = "Rock Album", Price = 9.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId });
            await _context.SaveChangesAsync();

            var repo = new GenreRepository(_context);

            // Act
            var result = await repo.GetGenreWithAlbumsAsync("Rock");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Rock", result.Name);
            Assert.Single(result.Albums);
        }

        // ─── CartRepository ──────────────────────────────────────────────────────────

        [Fact]
        public async Task CartRepository_GetCartItemsAsync_ReturnsCartItems()
        {
            // Arrange
            var genre = new Genre { Name = "Rock", Description = "Rock" };
            var artist = new Artist { Name = "Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();

            var album = new Album { Title = "Album", Price = 9.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var cartId = "test-cart";
            _context.Carts.Add(new Cart { CartId = cartId, AlbumId = album.AlbumId, Count = 2, DateCreated = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            var repo = new CartRepository(_context);

            // Act
            var result = await repo.GetCartItemsAsync(cartId);

            // Assert
            Assert.Single(result);
            Assert.Equal(cartId, result[0].CartId);
        }

        [Fact]
        public async Task CartRepository_GetCartCountAsync_ReturnsCorrectCount()
        {
            // Arrange
            var genre = new Genre { Name = "Rock", Description = "Rock" };
            var artist = new Artist { Name = "Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();

            var album = new Album { Title = "Album", Price = 9.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var cartId = "test-cart";
            _context.Carts.AddRange(
                new Cart { CartId = cartId, AlbumId = album.AlbumId, Count = 2, DateCreated = DateTime.UtcNow },
                new Cart { CartId = cartId, AlbumId = album.AlbumId, Count = 3, DateCreated = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            var repo = new CartRepository(_context);

            // Act
            var result = await repo.GetCartCountAsync(cartId);

            // Assert
            Assert.Equal(5, result);
        }

        [Fact]
        public async Task CartRepository_GetCartCountAsync_EmptyCart_ReturnsZero()
        {
            // Arrange
            var repo = new CartRepository(_context);

            // Act
            var result = await repo.GetCartCountAsync("empty-cart");

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task CartRepository_EmptyCartAsync_RemovesAllItems()
        {
            // Arrange
            var genre = new Genre { Name = "Rock", Description = "Rock" };
            var artist = new Artist { Name = "Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();

            var album = new Album { Title = "Album", Price = 9.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var cartId = "test-cart";
            _context.Carts.Add(new Cart { CartId = cartId, AlbumId = album.AlbumId, Count = 2, DateCreated = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            var repo = new CartRepository(_context);

            // Act
            await repo.EmptyCartAsync(cartId);

            // Assert
            Assert.Equal(0, _context.Carts.Count(c => c.CartId == cartId));
        }

        [Fact]
        public async Task CartRepository_MigrateCartAsync_UpdatesCartId()
        {
            // Arrange
            var genre = new Genre { Name = "Rock", Description = "Rock" };
            var artist = new Artist { Name = "Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();

            var album = new Album { Title = "Album", Price = 9.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var oldCartId = "old-cart";
            var newCartId = "new-cart";
            _context.Carts.Add(new Cart { CartId = oldCartId, AlbumId = album.AlbumId, Count = 1, DateCreated = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            var repo = new CartRepository(_context);

            // Act
            await repo.MigrateCartAsync(oldCartId, newCartId);

            // Assert
            Assert.Equal(0, _context.Carts.Count(c => c.CartId == oldCartId));
            Assert.Equal(1, _context.Carts.Count(c => c.CartId == newCartId));
        }

        // ─── OrderRepository ─────────────────────────────────────────────────────────

        [Fact]
        public async Task OrderRepository_ValidateOrderOwnerAsync_ValidOwner_ReturnsTrue()
        {
            // Arrange
            var order = new Order
            {
                Username = "testuser",
                OrderDate = DateTime.UtcNow,
                FirstName = "John", LastName = "Doe",
                Address = "123 Main", City = "City", State = "ST",
                PostalCode = "12345", Country = "USA",
                Phone = "555-1234", Email = "john@example.com"
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var repo = new OrderRepository(_context);

            // Act
            var result = await repo.ValidateOrderOwnerAsync(order.OrderId, "testuser");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task OrderRepository_ValidateOrderOwnerAsync_InvalidOwner_ReturnsFalse()
        {
            // Arrange
            var order = new Order
            {
                Username = "testuser",
                OrderDate = DateTime.UtcNow,
                FirstName = "John", LastName = "Doe",
                Address = "123 Main", City = "City", State = "ST",
                PostalCode = "12345", Country = "USA",
                Phone = "555-1234", Email = "john@example.com"
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var repo = new OrderRepository(_context);

            // Act
            var result = await repo.ValidateOrderOwnerAsync(order.OrderId, "wronguser");

            // Assert
            Assert.False(result);
        }
    }
}
