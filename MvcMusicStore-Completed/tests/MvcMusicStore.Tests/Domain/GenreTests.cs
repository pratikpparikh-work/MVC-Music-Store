using MvcMusicStore.Domain.Entities;
using Xunit;

namespace MvcMusicStore.Tests.Domain
{
    public class GenreTests
    {
        [Fact]
        public void Genre_DefaultConstructor_InitializesProperties()
        {
            // Arrange & Act
            var genre = new Genre();

            // Assert
            Assert.Equal(0, genre.GenreId);
            Assert.Equal(string.Empty, genre.Name);
            Assert.Equal(string.Empty, genre.Description);
            Assert.NotNull(genre.Albums);
            Assert.Empty(genre.Albums);
        }

        [Fact]
        public void Genre_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var genre = new Genre
            {
                GenreId = 1,
                Name = "Rock",
                Description = "Rock music genre"
            };

            // Assert
            Assert.Equal(1, genre.GenreId);
            Assert.Equal("Rock", genre.Name);
            Assert.Equal("Rock music genre", genre.Description);
        }

        [Fact]
        public void Genre_Albums_CanAddAlbums()
        {
            // Arrange
            var genre = new Genre { GenreId = 1, Name = "Rock" };
            var album = new Album { AlbumId = 1, Title = "Test Album", GenreId = 1 };

            // Act
            genre.Albums.Add(album);

            // Assert
            Assert.Single(genre.Albums);
            Assert.Equal(album, genre.Albums[0]);
        }

        [Fact]
        public void Genre_Albums_CanAddMultipleAlbums()
        {
            // Arrange
            var genre = new Genre { GenreId = 1, Name = "Rock" };
            var album1 = new Album { AlbumId = 1, Title = "Album 1", GenreId = 1 };
            var album2 = new Album { AlbumId = 2, Title = "Album 2", GenreId = 1 };

            // Act
            genre.Albums.Add(album1);
            genre.Albums.Add(album2);

            // Assert
            Assert.Equal(2, genre.Albums.Count);
        }

        [Fact]
        public void Genre_Description_CanBeEmpty()
        {
            // Arrange & Act
            var genre = new Genre { GenreId = 1, Name = "Rock", Description = string.Empty };

            // Assert
            Assert.Equal(string.Empty, genre.Description);
        }

        [Fact]
        public void Genre_Name_CanBeUpdated()
        {
            // Arrange
            var genre = new Genre { GenreId = 1, Name = "Rock" };

            // Act
            genre.Name = "Jazz";

            // Assert
            Assert.Equal("Jazz", genre.Name);
        }
    }
}
