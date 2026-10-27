using Moq;
using MvcMusicStore.Application.DTOs;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Application.Services;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Domain.Interfaces;
using Xunit;

namespace MvcMusicStore.Tests.Application
{
    public class AlbumServiceTests
    {
        private readonly Mock<IAlbumRepository> _albumRepoMock;
        private readonly Mock<IGenreRepository> _genreRepoMock;
        private readonly Mock<IArtistRepository> _artistRepoMock;
        private readonly AlbumService _service;

        public AlbumServiceTests()
        {
            _albumRepoMock = new Mock<IAlbumRepository>();
            _genreRepoMock = new Mock<IGenreRepository>();
            _artistRepoMock = new Mock<IArtistRepository>();
            _service = new AlbumService(_albumRepoMock.Object, _genreRepoMock.Object, _artistRepoMock.Object);
        }

        [Fact]
        public async Task GetTopSellingAlbumsAsync_ReturnsAlbumDtos()
        {
            // Arrange
            var albums = new List<Album>
            {
                new Album { AlbumId = 1, Title = "Album 1", Price = 9.99m, GenreId = 1, ArtistId = 1,
                    Genre = new Genre { Name = "Rock" }, Artist = new Artist { Name = "Artist 1" } },
                new Album { AlbumId = 2, Title = "Album 2", Price = 14.99m, GenreId = 2, ArtistId = 2,
                    Genre = new Genre { Name = "Jazz" }, Artist = new Artist { Name = "Artist 2" } }
            };
            _albumRepoMock.Setup(r => r.GetTopSellingAlbumsAsync(5)).ReturnsAsync(albums);

            // Act
            var result = await _service.GetTopSellingAlbumsAsync(5);

            // Assert
            var list = result.ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal("Album 1", list[0].Title);
            Assert.Equal("Rock", list[0].GenreName);
            Assert.Equal("Artist 1", list[0].ArtistName);
        }

        [Fact]
        public async Task GetTopSellingAlbumsAsync_EmptyList_ReturnsEmpty()
        {
            // Arrange
            _albumRepoMock.Setup(r => r.GetTopSellingAlbumsAsync(5)).ReturnsAsync(new List<Album>());

            // Act
            var result = await _service.GetTopSellingAlbumsAsync(5);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAlbumsAsync_ReturnsAllAlbumDtos()
        {
            // Arrange
            var albums = new List<Album>
            {
                new Album { AlbumId = 1, Title = "Album 1", Price = 9.99m,
                    Genre = new Genre { Name = "Rock" }, Artist = new Artist { Name = "Artist 1" } }
            };
            _albumRepoMock.Setup(r => r.GetAlbumsWithDetailsAsync()).ReturnsAsync(albums);

            // Act
            var result = await _service.GetAllAlbumsAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Album 1", result.First().Title);
        }

        [Fact]
        public async Task GetAlbumByIdAsync_ExistingId_ReturnsAlbumDto()
        {
            // Arrange
            var album = new Album { AlbumId = 1, Title = "Test Album", Price = 9.99m,
                Genre = new Genre { Name = "Rock" }, Artist = new Artist { Name = "Artist 1" } };
            _albumRepoMock.Setup(r => r.GetAlbumWithDetailsAsync(1)).ReturnsAsync(album);

            // Act
            var result = await _service.GetAlbumByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.AlbumId);
            Assert.Equal("Test Album", result.Title);
        }

        [Fact]
        public async Task GetAlbumByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            _albumRepoMock.Setup(r => r.GetAlbumWithDetailsAsync(999)).ReturnsAsync((Album?)null);

            // Act
            var result = await _service.GetAlbumByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAlbumAsync_ValidDto_CreatesAndReturnsDto()
        {
            // Arrange
            var dto = new AlbumDto { Title = "New Album", Price = 9.99m, GenreId = 1, ArtistId = 1 };
            _albumRepoMock.Setup(r => r.AddAsync(It.IsAny<Album>())).Returns(Task.CompletedTask);
            _albumRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.CreateAlbumAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Album", result.Title);
            _albumRepoMock.Verify(r => r.AddAsync(It.IsAny<Album>()), Times.Once);
            _albumRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAlbumAsync_ExistingAlbum_UpdatesSuccessfully()
        {
            // Arrange
            var existingAlbum = new Album { AlbumId = 1, Title = "Old Title", Price = 9.99m };
            var dto = new AlbumDto { AlbumId = 1, Title = "New Title", Price = 14.99m, GenreId = 2, ArtistId = 2, AlbumArtUrl = "/new.jpg" };
            _albumRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingAlbum);
            _albumRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _service.UpdateAlbumAsync(dto);

            // Assert
            Assert.Equal("New Title", existingAlbum.Title);
            Assert.Equal(14.99m, existingAlbum.Price);
            _albumRepoMock.Verify(r => r.Update(existingAlbum), Times.Once);
            _albumRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAlbumAsync_NonExistingAlbum_ThrowsKeyNotFoundException()
        {
            // Arrange
            _albumRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Album?)null);
            var dto = new AlbumDto { AlbumId = 999, Title = "Test" };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAlbumAsync(dto));
        }

        [Fact]
        public async Task DeleteAlbumAsync_ExistingAlbum_DeletesSuccessfully()
        {
            // Arrange
            var album = new Album { AlbumId = 1, Title = "Test Album" };
            _albumRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(album);
            _albumRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _service.DeleteAlbumAsync(1);

            // Assert
            _albumRepoMock.Verify(r => r.Remove(album), Times.Once);
            _albumRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAlbumAsync_NonExistingAlbum_ThrowsKeyNotFoundException()
        {
            // Arrange
            _albumRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Album?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAlbumAsync(999));
        }

        [Fact]
        public async Task GetAlbumByIdAsync_AlbumWithNullGenreAndArtist_ReturnsNullNames()
        {
            // Arrange
            var album = new Album { AlbumId = 1, Title = "Test Album", Price = 9.99m, Genre = null, Artist = null };
            _albumRepoMock.Setup(r => r.GetAlbumWithDetailsAsync(1)).ReturnsAsync(album);

            // Act
            var result = await _service.GetAlbumByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.GenreName);
            Assert.Null(result.ArtistName);
        }
    }
}
