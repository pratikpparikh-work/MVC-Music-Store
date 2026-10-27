using MvcMusicStore.Domain.Entities;
using Xunit;

namespace MvcMusicStore.Tests.Domain
{
    public class ArtistTests
    {
        [Fact]
        public void Artist_DefaultConstructor_InitializesProperties()
        {
            // Arrange & Act
            var artist = new Artist();

            // Assert
            Assert.Equal(0, artist.ArtistId);
            Assert.Equal(string.Empty, artist.Name);
            Assert.NotNull(artist.Albums);
            Assert.Empty(artist.Albums);
        }

        [Fact]
        public void Artist_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var artist = new Artist
            {
                ArtistId = 1,
                Name = "Test Artist"
            };

            // Assert
            Assert.Equal(1, artist.ArtistId);
            Assert.Equal("Test Artist", artist.Name);
        }

        [Fact]
        public void Artist_Albums_CanAddAlbums()
        {
            // Arrange
            var artist = new Artist { ArtistId = 1, Name = "Test Artist" };
            var album = new Album { AlbumId = 1, Title = "Test Album", ArtistId = 1 };

            // Act
            artist.Albums.Add(album);

            // Assert
            Assert.Single(artist.Albums);
            Assert.Equal(album, artist.Albums[0]);
        }

        [Fact]
        public void Artist_Albums_CanAddMultipleAlbums()
        {
            // Arrange
            var artist = new Artist { ArtistId = 1, Name = "Test Artist" };
            var album1 = new Album { AlbumId = 1, Title = "Album 1", ArtistId = 1 };
            var album2 = new Album { AlbumId = 2, Title = "Album 2", ArtistId = 1 };

            // Act
            artist.Albums.Add(album1);
            artist.Albums.Add(album2);

            // Assert
            Assert.Equal(2, artist.Albums.Count);
        }

        [Fact]
        public void Artist_Name_CanBeEmpty()
        {
            // Arrange & Act
            var artist = new Artist { ArtistId = 1, Name = string.Empty };

            // Assert
            Assert.Equal(string.Empty, artist.Name);
        }

        [Fact]
        public void Artist_Name_CanBeSet()
        {
            // Arrange
            var artist = new Artist();

            // Act
            artist.Name = "New Artist Name";

            // Assert
            Assert.Equal("New Artist Name", artist.Name);
        }
    }
}
