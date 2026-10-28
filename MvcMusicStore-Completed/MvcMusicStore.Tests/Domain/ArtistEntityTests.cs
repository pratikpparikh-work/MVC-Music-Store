using MvcMusicStore.Domain.Entities;
using Xunit;

namespace MvcMusicStore.Tests.Domain
{
    /// <summary>
    /// Unit tests for the Artist domain entity.
    /// </summary>
    public class ArtistEntityTests
    {
        [Fact]
        public void Artist_DefaultConstructor_InitializesProperties()
        {
            // Arrange & Act
            var artist = new Artist();

            // Assert
            Assert.Equal(0, artist.ArtistId);
            Assert.Equal(string.Empty, artist.Name);
        }

        [Fact]
        public void Artist_SetName_ReturnsCorrectName()
        {
            // Arrange
            var artist = new Artist();

            // Act
            artist.Name = "The Beatles";

            // Assert
            Assert.Equal("The Beatles", artist.Name);
        }

        [Fact]
        public void Artist_SetArtistId_ReturnsCorrectId()
        {
            // Arrange
            var artist = new Artist();

            // Act
            artist.ArtistId = 42;

            // Assert
            Assert.Equal(42, artist.ArtistId);
        }

        [Fact]
        public void Artist_SetAllProperties_AllPropertiesCorrect()
        {
            // Arrange & Act
            var artist = new Artist
            {
                ArtistId = 10,
                Name = "Led Zeppelin"
            };

            // Assert
            Assert.Equal(10, artist.ArtistId);
            Assert.Equal("Led Zeppelin", artist.Name);
        }

        [Fact]
        public void Artist_EmptyName_ReturnsEmptyString()
        {
            // Arrange
            var artist = new Artist { Name = string.Empty };

            // Assert
            Assert.Equal(string.Empty, artist.Name);
        }

        [Theory]
        [InlineData("Pink Floyd")]
        [InlineData("AC/DC")]
        [InlineData("Metallica")]
        public void Artist_SetVariousNames_ReturnsCorrectName(string name)
        {
            // Arrange
            var artist = new Artist();

            // Act
            artist.Name = name;

            // Assert
            Assert.Equal(name, artist.Name);
        }
    }
}
