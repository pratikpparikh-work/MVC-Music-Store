using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Application.Services;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Infrastructure.Data;
using MvcMusicStore.Infrastructure.Repositories;
using Xunit;

namespace MvcMusicStore.Tests.Services
{
    /// <summary>
    /// Unit tests for AlbumService using EF Core InMemory database.
    /// Tests the migration from EF6 to EF Core patterns.
    /// </summary>
    public class AlbumServiceTests : IDisposable
    {
        private readonly MusicStoreDbContext _context;
        private readonly AlbumRepository _albumRepository;
        private readonly GenreRepository _genreRepository;
        private readonly ArtistRepository _artistRepository;
        private readonly AlbumService _albumService;

        public AlbumServiceTests()
        {
            var options = new DbContextOptionsBuilder<MusicStoreDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MusicStoreDbContext(options);
            _albumRepository = new AlbumRepository(_context);
            _genreRepository = new GenreRepository(_context);
            _artistRepository = new ArtistRepository(_context);
            _albumService = new AlbumService(_albumRepository, _genreRepository, _artistRepository);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var genre = new Genre { GenreId = 1, Name = "Rock", Description = "Rock music" };
            var artist = new Artist { ArtistId = 1, Name = "Test Artist" };
            var album = new Album
            {
                AlbumId = 1,
                Title = "Test Album",
                Price = 9.99M,
                AlbumArtUrl = "/images/test.gif",
                GenreId = 1,
                ArtistId = 1
            };

            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            _context.Albums.Add(album);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllAlbumsAsync_ReturnsAllAlbums()
        {
            // Act
            var albums = await _albumService.GetAllAlbumsAsync();

            // Assert
            Assert.Single(albums);
        }

        [Fact]
        public async Task GetAlbumByIdAsync_ReturnsAlbum_WhenExists()
        {
            // Act
            var album = await _albumService.GetAlbumByIdAsync(1);

            // Assert
            Assert.NotNull(album);
            Assert.Equal("Test Album", album.Title);
        }

        [Fact]
        public async Task GetAlbumByIdAsync_ReturnsNull_WhenNotExists()
        {
            // Act
            var album = await _albumService.GetAlbumByIdAsync(999);

            // Assert
            Assert.Null(album);
        }

        [Fact]
        public async Task CreateAlbumAsync_AddsAlbum()
        {
            // Arrange
            var newAlbum = new MvcMusicStore.Application.DTOs.AlbumDto
            {
                Title = "New Album",
                Price = 7.99M,
                AlbumArtUrl = "/images/new.gif",
                GenreId = 1,
                ArtistId = 1
            };

            // Act
            var result = await _albumService.CreateAlbumAsync(newAlbum);

            // Assert
            Assert.True(result.AlbumId > 0);
            var albums = await _albumService.GetAllAlbumsAsync();
            Assert.Equal(2, albums.Count());
        }

        [Fact]
        public async Task DeleteAlbumAsync_RemovesAlbum()
        {
            // Act
            await _albumService.DeleteAlbumAsync(1);

            // Assert
            var album = await _albumService.GetAlbumByIdAsync(1);
            Assert.Null(album);
        }

        [Fact]
        public async Task UpdateAlbumAsync_UpdatesAlbum()
        {
            // Arrange
            var albumDto = new MvcMusicStore.Application.DTOs.AlbumDto
            {
                AlbumId = 1,
                Title = "Updated Album",
                Price = 12.99M,
                AlbumArtUrl = "/images/updated.gif",
                GenreId = 1,
                ArtistId = 1
            };

            // Act
            await _albumService.UpdateAlbumAsync(albumDto);

            // Assert
            var updated = await _albumService.GetAlbumByIdAsync(1);
            Assert.NotNull(updated);
            Assert.Equal("Updated Album", updated.Title);
            Assert.Equal(12.99M, updated.Price);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

    /// <summary>
    /// Unit tests for ShoppingCartService.
    /// Tests session-based cart operations migrated from legacy ShoppingCart model.
    /// </summary>
    public class ShoppingCartServiceTests : IDisposable
    {
        private readonly MusicStoreDbContext _context;
        private readonly CartRepository _cartRepository;
        private readonly AlbumRepository _albumRepository;
        private readonly OrderDetailRepository _orderDetailRepository;
        private readonly OrderRepository _orderRepository;
        private readonly ShoppingCartService _cartService;

        private const string TestCartId = "test-cart-123";

        public ShoppingCartServiceTests()
        {
            var options = new DbContextOptionsBuilder<MusicStoreDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MusicStoreDbContext(options);
            _cartRepository = new CartRepository(_context);
            _albumRepository = new AlbumRepository(_context);
            _orderDetailRepository = new OrderDetailRepository(_context);
            _orderRepository = new OrderRepository(_context);
            _cartService = new ShoppingCartService(
                _cartRepository, _albumRepository, _orderDetailRepository, _orderRepository);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var genre = new Genre { GenreId = 1, Name = "Rock", Description = "Rock music" };
            var artist = new Artist { ArtistId = 1, Name = "Test Artist" };
            var album = new Album
            {
                AlbumId = 1,
                Title = "Test Album",
                Price = 9.99M,
                AlbumArtUrl = "/images/test.gif",
                GenreId = 1,
                ArtistId = 1
            };

            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            _context.Albums.Add(album);
            _context.SaveChanges();
        }

        [Fact]
        public async Task AddToCartAsync_AddsNewItem_WhenNotInCart()
        {
            // Act
            await _cartService.AddToCartAsync(TestCartId, 1);

            // Assert
            var cart = await _cartService.GetCartAsync(TestCartId);
            Assert.Single(cart.CartItems);
            Assert.Equal(1, cart.CartItems[0].Count);
        }

        [Fact]
        public async Task AddToCartAsync_IncrementsCount_WhenAlreadyInCart()
        {
            // Arrange
            await _cartService.AddToCartAsync(TestCartId, 1);

            // Act
            await _cartService.AddToCartAsync(TestCartId, 1);

            // Assert
            var cart = await _cartService.GetCartAsync(TestCartId);
            Assert.Single(cart.CartItems);
            Assert.Equal(2, cart.CartItems[0].Count);
        }

        [Fact]
        public async Task GetCartCountAsync_ReturnsCorrectCount()
        {
            // Arrange
            await _cartService.AddToCartAsync(TestCartId, 1);
            await _cartService.AddToCartAsync(TestCartId, 1);

            // Act
            var count = await _cartService.GetCartCountAsync(TestCartId);

            // Assert
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetCartTotalAsync_ReturnsCorrectTotal()
        {
            // Arrange
            await _cartService.AddToCartAsync(TestCartId, 1); // 9.99

            // Act
            var total = await _cartService.GetCartTotalAsync(TestCartId);

            // Assert
            Assert.Equal(9.99M, total);
        }

        [Fact]
        public async Task RemoveFromCartAsync_RemovesItem_WhenCountIsOne()
        {
            // Arrange
            await _cartService.AddToCartAsync(TestCartId, 1);
            var cart = await _cartService.GetCartAsync(TestCartId);
            var recordId = cart.CartItems[0].RecordId;

            // Act
            await _cartService.RemoveFromCartAsync(TestCartId, recordId);

            // Assert
            var updatedCart = await _cartService.GetCartAsync(TestCartId);
            Assert.Empty(updatedCart.CartItems);
        }

        [Fact]
        public async Task MigrateCartAsync_TransfersCartToNewId()
        {
            // Arrange
            await _cartService.AddToCartAsync(TestCartId, 1);
            const string newCartId = "new-user-cart";

            // Act
            await _cartService.MigrateCartAsync(TestCartId, newCartId);

            // Assert
            var oldCart = await _cartService.GetCartAsync(TestCartId);
            var newCart = await _cartService.GetCartAsync(newCartId);
            Assert.Empty(oldCart.CartItems);
            Assert.Single(newCart.CartItems);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

    /// <summary>
    /// Unit tests for GenreService.
    /// </summary>
    public class GenreServiceTests : IDisposable
    {
        private readonly MusicStoreDbContext _context;
        private readonly GenreRepository _genreRepository;
        private readonly GenreService _genreService;

        public GenreServiceTests()
        {
            var options = new DbContextOptionsBuilder<MusicStoreDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MusicStoreDbContext(options);
            _genreRepository = new GenreRepository(_context);
            _genreService = new GenreService(_genreRepository);

            SeedTestData();
        }

        private void SeedTestData()
        {
            _context.Genres.AddRange(
                new Genre { GenreId = 1, Name = "Rock", Description = "Rock music" },
                new Genre { GenreId = 2, Name = "Jazz", Description = "Jazz music" }
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllGenresAsync_ReturnsAllGenres()
        {
            // Act
            var genres = await _genreService.GetAllGenresAsync();

            // Assert
            Assert.Equal(2, genres.Count());
        }

        [Fact]
        public async Task GetGenreByNameAsync_ReturnsGenre_WhenExists()
        {
            // Act
            var genre = await _genreService.GetGenreByNameAsync("Rock");

            // Assert
            Assert.NotNull(genre);
            Assert.Equal("Rock", genre.Name);
        }

        [Fact]
        public async Task GetGenreByNameAsync_ReturnsNull_WhenNotExists()
        {
            // Act
            var genre = await _genreService.GetGenreByNameAsync("NonExistent");

            // Assert
            Assert.Null(genre);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
