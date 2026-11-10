using System;
using System.Collections.Generic;
using Xunit;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Models
{
    public class GenreTests
    {
        [Fact]
        public void Genre_DefaultConstructor_SetsDefaultValues()
        {
            var genre = new Genre();
            Assert.Equal(0, genre.GenreId);
            Assert.Equal(string.Empty, genre.Name);
            Assert.Null(genre.Description);
            Assert.NotNull(genre.Albums);
            Assert.Empty(genre.Albums);
        }

        [Fact]
        public void Genre_SetGenreId_ReturnsCorrectValue()
        {
            var genre = new Genre();
            genre.GenreId = 5;
            Assert.Equal(5, genre.GenreId);
        }

        [Fact]
        public void Genre_SetName_ReturnsCorrectValue()
        {
            var genre = new Genre();
            genre.Name = "Rock";
            Assert.Equal("Rock", genre.Name);
        }

        [Fact]
        public void Genre_SetDescription_ReturnsCorrectValue()
        {
            var genre = new Genre();
            genre.Description = "Rock music genre";
            Assert.Equal("Rock music genre", genre.Description);
        }

        [Fact]
        public void Genre_SetDescriptionToNull_ReturnsNull()
        {
            var genre = new Genre { Description = "Some description" };
            genre.Description = null;
            Assert.Null(genre.Description);
        }

        [Fact]
        public void Genre_AddAlbumToList_AlbumIsInList()
        {
            var genre = new Genre { GenreId = 1, Name = "Rock" };
            var album = new Album { AlbumId = 1, Title = "Test Album" };
            genre.Albums.Add(album);
            Assert.Single(genre.Albums);
            Assert.Equal("Test Album", genre.Albums[0].Title);
        }

        [Fact]
        public void Genre_AlbumsList_CanHoldMultipleAlbums()
        {
            var genre = new Genre { GenreId = 1, Name = "Rock" };
            genre.Albums.Add(new Album { AlbumId = 1, Title = "Album 1" });
            genre.Albums.Add(new Album { AlbumId = 2, Title = "Album 2" });
            genre.Albums.Add(new Album { AlbumId = 3, Title = "Album 3" });
            Assert.Equal(3, genre.Albums.Count);
        }

        [Fact]
        public void Genre_InitializeWithValues_PropertiesAreSet()
        {
            var genre = new Genre
            {
                GenreId = 10,
                Name = "Jazz",
                Description = "Jazz music"
            };
            Assert.Equal(10, genre.GenreId);
            Assert.Equal("Jazz", genre.Name);
            Assert.Equal("Jazz music", genre.Description);
        }

        [Fact]
        public void Genre_SetNameToEmpty_ReturnsEmptyString()
        {
            var genre = new Genre { Name = "Rock" };
            genre.Name = string.Empty;
            Assert.Equal(string.Empty, genre.Name);
        }

        [Theory]
        [InlineData("Rock")]
        [InlineData("Jazz")]
        [InlineData("Metal")]
        [InlineData("Classical")]
        [InlineData("Pop")]
        public void Genre_SetVariousNames_ReturnsCorrectValue(string name)
        {
            var genre = new Genre();
            genre.Name = name;
            Assert.Equal(name, genre.Name);
        }

        [Fact]
        public void Genre_RemoveAlbumFromList_AlbumIsRemoved()
        {
            var genre = new Genre { GenreId = 1, Name = "Rock" };
            var album = new Album { AlbumId = 1, Title = "Test Album" };
            genre.Albums.Add(album);
            genre.Albums.Remove(album);
            Assert.Empty(genre.Albums);
        }
    }
}
