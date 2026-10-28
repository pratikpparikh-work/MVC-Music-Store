using System.Collections.Generic;
using MvcMusicStore.Domain.Entities;
using Xunit;

namespace MvcMusicStore.Tests.Domain
{
    /// <summary>
    /// Unit tests for the Genre domain entity.
    /// </summary>
    public class GenreEntityTests
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
        public void Genre_SetName_ReturnsCorrectName()
        {
            // Arrange
            var genre = new Genre();

            // Act
            genre.Name = "Rock";

            // Assert
            Assert.Equal("Rock", genre.Name);
        }

        [Fact]
        public void Genre_SetDescription_ReturnsCorrectDescription()
        {
            // Arrange
            var genre = new Genre();

            // Act
            genre.Description = "Rock music genre";

            // Assert
            Assert.Equal("Rock music genre", genre.Description);
        }

        [Fact]
        public void Genre_SetGenreId_ReturnsCorrectId()
        {
            // Arrange
            var genre = new Genre();

            // Act
            genre.GenreId = 7;

            // Assert
            Assert.Equal(7, genre.GenreId);
        }

        [Fact]
        public void Genre_AddAlbum_AlbumsContainsItem()
        {
            // Arrange
            var genre = new Genre { GenreId = 1, Name = "Jazz" };
            var album = new Album { AlbumId = 1, Title = "Jazz Album" };

            // Act
            genre.Albums.Add(album);

            // Assert
            Assert.Single(genre.Albums);
            Assert.Equal("Jazz Album", genre.Albums[0].Title);
        }

        [Fact]
        public void Genre_AddMultipleAlbums_AlbumsContainsAllItems()
        {
            // Arrange
            var genre = new Genre { GenreId = 1, Name = "Pop" };

            // Act
            genre.Albums.Add(new Album { AlbumId = 1, Title = "Pop Album 1" });
            genre.Albums.Add(new Album { AlbumId = 2, Title = "Pop Album 2" });
            genre.Albums.Add(new Album { AlbumId = 3, Title = "Pop Album 3" });

            // Assert
            Assert.Equal(3, genre.Albums.Count);
        }

        [Fact]
        public void Genre_SetAllProperties_AllPropertiesCorrect()
        {
            // Arrange & Act
            var genre = new Genre
            {
                GenreId = 5,
                Name = "Classical",
                Description = "Classical music"
            };

            // Assert
            Assert.Equal(5, genre.GenreId);
            Assert.Equal("Classical", genre.Name);
            Assert.Equal("Classical music", genre.Description);
        }
    }
}
