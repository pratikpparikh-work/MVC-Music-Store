using Moq;
using MvcMusicStore.Application.Services;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Domain.Interfaces;
using Xunit;

namespace MvcMusicStore.Tests.Application
{
    public class ArtistServiceTests
    {
        private readonly Mock<IArtistRepository> _artistRepoMock;
        private readonly ArtistService _service;

        public ArtistServiceTests()
        {
            _artistRepoMock = new Mock<IArtistRepository>();
            _service = new ArtistService(_artistRepoMock.Object);
        }

        [Fact]
        public async Task GetAllArtistsAsync_ReturnsAllArtistDtos()
        {
            // Arrange
            var artists = new List<Artist>
            {
                new Artist { ArtistId = 1, Name = "Artist One" },
                new Artist { ArtistId = 2, Name = "Artist Two" },
                new Artist { ArtistId = 3, Name = "Artist Three" }
            };
            _artistRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(artists);

            // Act
            var result = await _service.GetAllArtistsAsync();

            // Assert
            var list = result.ToList();
            Assert.Equal(3, list.Count);
            Assert.Equal(1, list[0].ArtistId);
            Assert.Equal("Artist One", list[0].Name);
            Assert.Equal(2, list[1].ArtistId);
            Assert.Equal("Artist Two", list[1].Name);
        }

        [Fact]
        public async Task GetAllArtistsAsync_EmptyList_ReturnsEmpty()
        {
            // Arrange
            _artistRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Artist>());

            // Act
            var result = await _service.GetAllArtistsAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllArtistsAsync_MapsArtistIdCorrectly()
        {
            // Arrange
            var artists = new List<Artist>
            {
                new Artist { ArtistId = 42, Name = "Special Artist" }
            };
            _artistRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(artists);

            // Act
            var result = await _service.GetAllArtistsAsync();

            // Assert
            var dto = result.First();
            Assert.Equal(42, dto.ArtistId);
            Assert.Equal("Special Artist", dto.Name);
        }
    }
}
