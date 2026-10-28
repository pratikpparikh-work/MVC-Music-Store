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
    /// Unit tests for ArtistService.
    /// </summary>
    public class ArtistServiceTests
    {
        private readonly Mock<IArtistRepository> _mockArtistRepository;
        private readonly IArtistService _artistService;

        public ArtistServiceTests()
        {
            _mockArtistRepository = new Mock<IArtistRepository>();
            _artistService = new ArtistService(_mockArtistRepository.Object);
        }

        [Fact]
        public async Task GetAllArtistsAsync_ReturnsAllArtists()
        {
            // Arrange
            var expectedArtists = new List<Artist>
            {
                new Artist { ArtistId = 1, Name = "The Beatles" },
                new Artist { ArtistId = 2, Name = "Led Zeppelin" },
                new Artist { ArtistId = 3, Name = "Pink Floyd" }
            };
            _mockArtistRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(expectedArtists);

            // Act
            var result = await _artistService.GetAllArtistsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetAllArtistsAsync_CallsRepositoryOnce()
        {
            // Arrange
            _mockArtistRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Artist>());

            // Act
            await _artistService.GetAllArtistsAsync();

            // Assert
            _mockArtistRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetArtistByIdAsync_WithValidId_ReturnsArtist()
        {
            // Arrange
            var expectedArtist = new Artist { ArtistId = 1, Name = "The Beatles" };
            _mockArtistRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(expectedArtist);

            // Act
            var result = await _artistService.GetArtistByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ArtistId);
            Assert.Equal("The Beatles", result.Name);
        }

        [Fact]
        public async Task GetArtistByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            _mockArtistRepository.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Artist?)null);

            // Act
            var result = await _artistService.GetArtistByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllArtistsAsync_WhenEmpty_ReturnsEmptyCollection()
        {
            // Arrange
            _mockArtistRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Artist>());

            // Act
            var result = await _artistService.GetAllArtistsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(100)]
        public async Task GetArtistByIdAsync_WithVariousIds_CallsRepositoryWithCorrectId(int artistId)
        {
            // Arrange
            _mockArtistRepository.Setup(r => r.GetByIdAsync(artistId))
                .ReturnsAsync(new Artist { ArtistId = artistId, Name = $"Artist {artistId}" });

            // Act
            var result = await _artistService.GetArtistByIdAsync(artistId);

            // Assert
            _mockArtistRepository.Verify(r => r.GetByIdAsync(artistId), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(artistId, result.ArtistId);
        }
    }
}
