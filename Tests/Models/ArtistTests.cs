using System;
using Xunit;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Models
{
    public class ArtistTests
    {
        [Fact]
        public void Artist_DefaultConstructor_SetsDefaultValues()
        {
            var artist = new Artist();
            Assert.Equal(0, artist.ArtistId);
            Assert.Equal(string.Empty, artist.Name);
        }

        [Fact]
        public void Artist_SetArtistId_ReturnsCorrectValue()
        {
            var artist = new Artist();
            artist.ArtistId = 7;
            Assert.Equal(7, artist.ArtistId);
        }

        [Fact]
        public void Artist_SetName_ReturnsCorrectValue()
        {
            var artist = new Artist();
            artist.Name = "Metallica";
            Assert.Equal("Metallica", artist.Name);
        }

        [Fact]
        public void Artist_SetNameToEmpty_ReturnsEmptyString()
        {
            var artist = new Artist { Name = "Metallica" };
            artist.Name = string.Empty;
            Assert.Equal(string.Empty, artist.Name);
        }

        [Fact]
        public void Artist_InitializeWithValues_PropertiesAreSet()
        {
            var artist = new Artist
            {
                ArtistId = 100,
                Name = "Led Zeppelin"
            };
            Assert.Equal(100, artist.ArtistId);
            Assert.Equal("Led Zeppelin", artist.Name);
        }

        [Fact]
        public void Artist_SetNameWithSpecialCharacters_ReturnsCorrectValue()
        {
            var artist = new Artist();
            artist.Name = "Guns N' Roses";
            Assert.Equal("Guns N' Roses", artist.Name);
        }

        [Fact]
        public void Artist_SetNameWithUnicodeCharacters_ReturnsCorrectValue()
        {
            var artist = new Artist();
            artist.Name = "Antônio Carlos Jobim";
            Assert.Equal("Antônio Carlos Jobim", artist.Name);
        }

        [Theory]
        [InlineData(1, "AC/DC")]
        [InlineData(2, "Nirvana")]
        [InlineData(3, "Pink Floyd")]
        public void Artist_SetIdAndName_ReturnsCorrectValues(int id, string name)
        {
            var artist = new Artist { ArtistId = id, Name = name };
            Assert.Equal(id, artist.ArtistId);
            Assert.Equal(name, artist.Name);
        }

        [Fact]
        public void Artist_SetArtistIdToZero_ReturnsZero()
        {
            var artist = new Artist { ArtistId = 5 };
            artist.ArtistId = 0;
            Assert.Equal(0, artist.ArtistId);
        }

        [Fact]
        public void Artist_SetArtistIdToNegative_ReturnsNegativeValue()
        {
            var artist = new Artist();
            artist.ArtistId = -1;
            Assert.Equal(-1, artist.ArtistId);
        }
    }
}
