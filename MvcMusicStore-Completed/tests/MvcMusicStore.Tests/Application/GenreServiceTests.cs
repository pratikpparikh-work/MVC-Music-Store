using Moq;
using MvcMusicStore.Application.Services;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Domain.Interfaces;
using Xunit;

namespace MvcMusicStore.Tests.Application
{
    public class GenreServiceTests
    {
        private readonly Mock<IGenreRepository> _genreRepoMock;
        private readonly GenreService _service;

        public GenreServiceTests()
        {
            _genreRepoMock = new Mock<IGenreRepository>();
            _service = new GenreService(_genreRepoMock.Object);
        }

        [Fact]
        public async Task GetAllGenresAsync_ReturnsAllGenreDtos()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new Genre { GenreId = 1, Name = "Rock", Description = "Rock music" },
                new Genre { GenreId = 2, Name = "Jazz", Description = "Jazz music" }
            };
            _genreRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(genres);

            // Act
            var result = await _service.GetAllGenresAsync();

            // Assert
            var list = result.ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal("Rock", list[0].Name);
            Assert.Equal("Jazz", list[1].Name);
        }

        [Fact]
        public async Task GetAllGenresAsync_EmptyList_ReturnsEmpty()
        {
            // Arrange
            _genreRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Genre>());

            // Act
            var result = await _service.GetAllGenresAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetGenreByNameAsync_ExistingName_ReturnsGenreDto()
        {
            // Arrange
            var genre = new Genre { GenreId = 1, Name = "Rock", Description = "Rock music" };
            _genreRepoMock.Setup(r => r.GetGenreByNameAsync("Rock")).ReturnsAsync(genre);

            // Act
            var result = await _service.GetGenreByNameAsync("Rock");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.GenreId);
            Assert.Equal("Rock", result.Name);
            Assert.Equal("Rock music", result.Description);
        }

        [Fact]
        public async Task GetGenreByNameAsync_NonExistingName_ReturnsNull()
        {
            // Arrange
            _genreRepoMock.Setup(r => r.GetGenreByNameAsync("Unknown")).ReturnsAsync((Genre?)null);

            // Act
            var result = await _service.GetGenreByNameAsync("Unknown");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetGenreWithAlbumsAsync_ExistingName_ReturnsGenreDtoWithAlbums()
        {
            // Arrange
            var genre = new Genre
            {
                GenreId = 1,
                Name = "Rock",
                Description = "Rock music",
                Albums = new List<Album>
                {
                    new Album { AlbumId = 1, Title = "Rock Album 1", Price = 9.99m, AlbumArtUrl = "/img/1.jpg",
                        Artist = new Artist { Name = "Artist 1" } },
                    new Album { AlbumId = 2, Title = "Rock Album 2", Price = 14.99m, AlbumArtUrl = "/img/2.jpg",
                        Artist = new Artist { Name = "Artist 2" } }
                }
            };
            _genreRepoMock.Setup(r => r.GetGenreWithAlbumsAsync("Rock")).ReturnsAsync(genre);

            // Act
            var result = await _service.GetGenreWithAlbumsAsync("Rock");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Rock", result.Name);
            Assert.Equal(2, result.Albums.Count);
            Assert.Equal("Rock Album 1", result.Albums[0].Title);
            Assert.Equal("Artist 1", result.Albums[0].ArtistName);
        }

        [Fact]
        public async Task GetGenreWithAlbumsAsync_NonExistingName_ReturnsNull()
        {
            // Arrange
            _genreRepoMock.Setup(r => r.GetGenreWithAlbumsAsync("Unknown")).ReturnsAsync((Genre?)null);

            // Act
            var result = await _service.GetGenreWithAlbumsAsync("Unknown");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetGenreWithAlbumsAsync_AlbumWithNullArtist_ReturnsNullArtistName()
        {
            // Arrange
            var genre = new Genre
            {
                GenreId = 1,
                Name = "Rock",
                Description = "Rock music",
                Albums = new List<Album>
                {
                    new Album { AlbumId = 1, Title = "Rock Album", Price = 9.99m, Artist = null }
                }
            };
            _genreRepoMock.Setup(r => r.GetGenreWithAlbumsAsync("Rock")).ReturnsAsync(genre);

            // Act
            var result = await _service.GetGenreWithAlbumsAsync("Rock");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Albums);
            Assert.Null(result.Albums[0].ArtistName);
        }
    }
}
