using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Application.Services;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Domain.Interfaces;
using Xunit;

namespace MvcMusicStore.Tests.Services
{
    /// <summary>
    /// Unit tests for AlbumService.
    /// Tests the application service layer in isolation using Moq.
    /// </summary>
    public class AlbumServiceTests
    {
        private readonly Mock<IAlbumRepository> _mockAlbumRepository;
        private readonly IAlbumService _albumService;

        public AlbumServiceTests()
        {
            _mockAlbumRepository = new Mock<IAlbumRepository>();
            _albumService = new AlbumService(_mockAlbumRepository.Object);
        }

        [Fact]
        public async Task GetAllAlbumsAsync_ReturnsAllAlbums()
        {
            // Arrange
            var expectedAlbums = new List<Album>
            {
                new Album { AlbumId = 1, Title = "Test Album 1", Price = 9.99M },
                new Album { AlbumId = 2, Title = "Test Album 2", Price = 8.99M }
            };
            _mockAlbumRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(expectedAlbums);

            // Act
            var result = await _albumService.GetAllAlbumsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAlbumByIdAsync_WithValidId_ReturnsAlbum()
        {
            // Arrange
            var expectedAlbum = new Album { AlbumId = 1, Title = "Test Album", Price = 9.99M };
            _mockAlbumRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(expectedAlbum);

            // Act
            var result = await _albumService.GetAlbumByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Album", result.Title);
        }

        [Fact]
        public async Task GetAlbumByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            _mockAlbumRepository.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Album?)null);

            // Act
            var result = await _albumService.GetAlbumByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetTopSellingAlbumsAsync_ReturnsCorrectCount()
        {
            // Arrange
            var albums = new List<Album>
            {
                new Album { AlbumId = 1, Title = "Top Album 1" },
                new Album { AlbumId = 2, Title = "Top Album 2" },
                new Album { AlbumId = 3, Title = "Top Album 3" }
            };
            _mockAlbumRepository.Setup(r => r.GetTopSellingAsync(3))
                .ReturnsAsync(albums);

            // Act
            var result = await _albumService.GetTopSellingAlbumsAsync(3);

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task CreateAlbumAsync_CallsRepositoryAdd()
        {
            // Arrange
            var newAlbum = new Album { Title = "New Album", Price = 9.99M };
            _mockAlbumRepository.Setup(r => r.AddAsync(newAlbum))
                .ReturnsAsync(newAlbum);

            // Act
            var result = await _albumService.CreateAlbumAsync(newAlbum);

            // Assert
            _mockAlbumRepository.Verify(r => r.AddAsync(newAlbum), Times.Once);
            Assert.Equal("New Album", result.Title);
        }

        [Fact]
        public async Task DeleteAlbumAsync_CallsRepositoryDelete()
        {
            // Arrange
            _mockAlbumRepository.Setup(r => r.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            await _albumService.DeleteAlbumAsync(1);

            // Assert
            _mockAlbumRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }
    }
}
