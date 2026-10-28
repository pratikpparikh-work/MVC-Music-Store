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
    /// Unit tests for GenreService.
    /// </summary>
    public class GenreServiceTests
    {
        private readonly Mock<IGenreRepository> _mockGenreRepository;
        private readonly IGenreService _genreService;

        public GenreServiceTests()
        {
            _mockGenreRepository = new Mock<IGenreRepository>();
            _genreService = new GenreService(_mockGenreRepository.Object);
        }

        [Fact]
        public async Task GetAllGenresAsync_ReturnsAllGenres()
        {
            // Arrange
            var expectedGenres = new List<Genre>
            {
                new Genre { GenreId = 1, Name = "Rock" },
                new Genre { GenreId = 2, Name = "Jazz" },
                new Genre { GenreId = 3, Name = "Pop" }
            };
            _mockGenreRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(expectedGenres);

            // Act
            var result = await _genreService.GetAllGenresAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetAllGenresAsync_CallsRepositoryOnce()
        {
            // Arrange
            _mockGenreRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Genre>());

            // Act
            await _genreService.GetAllGenresAsync();

            // Assert
            _mockGenreRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetGenreByNameAsync_WithValidName_ReturnsGenre()
        {
            // Arrange
            var expectedGenre = new Genre { GenreId = 1, Name = "Rock", Description = "Rock music" };
            _mockGenreRepository.Setup(r => r.GetByNameAsync("Rock"))
                .ReturnsAsync(expectedGenre);

            // Act
            var result = await _genreService.GetGenreByNameAsync("Rock");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Rock", result.Name);
            Assert.Equal(1, result.GenreId);
        }

        [Fact]
        public async Task GetGenreByNameAsync_WithInvalidName_ReturnsNull()
        {
            // Arrange
            _mockGenreRepository.Setup(r => r.GetByNameAsync("NonExistent"))
                .ReturnsAsync((Genre?)null);

            // Act
            var result = await _genreService.GetGenreByNameAsync("NonExistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetGenreByIdAsync_WithValidId_ReturnsGenre()
        {
            // Arrange
            var expectedGenre = new Genre { GenreId = 5, Name = "Classical" };
            _mockGenreRepository.Setup(r => r.GetByIdAsync(5))
                .ReturnsAsync(expectedGenre);

            // Act
            var result = await _genreService.GetGenreByIdAsync(5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.GenreId);
            Assert.Equal("Classical", result.Name);
        }

        [Fact]
        public async Task GetGenreByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            _mockGenreRepository.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Genre?)null);

            // Act
            var result = await _genreService.GetGenreByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllGenresAsync_WhenEmpty_ReturnsEmptyCollection()
        {
            // Arrange
            _mockGenreRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Genre>());

            // Act
            var result = await _genreService.GetAllGenresAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData("Rock")]
        [InlineData("Jazz")]
        [InlineData("Pop")]
        public async Task GetGenreByNameAsync_WithVariousNames_CallsRepositoryWithCorrectName(string genreName)
        {
            // Arrange
            _mockGenreRepository.Setup(r => r.GetByNameAsync(genreName))
                .ReturnsAsync(new Genre { Name = genreName });

            // Act
            var result = await _genreService.GetGenreByNameAsync(genreName);

            // Assert
            _mockGenreRepository.Verify(r => r.GetByNameAsync(genreName), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(genreName, result.Name);
        }
    }
}
