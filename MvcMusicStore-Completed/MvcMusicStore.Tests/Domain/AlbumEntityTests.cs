using System.Collections.Generic;
using MvcMusicStore.Domain.Entities;
using Xunit;

namespace MvcMusicStore.Tests.Domain
{
    /// <summary>
    /// Unit tests for the Album domain entity.
    /// </summary>
    public class AlbumEntityTests
    {
        [Fact]
        public void Album_DefaultConstructor_InitializesProperties()
        {
            // Arrange & Act
            var album = new Album();

            // Assert
            Assert.Equal(0, album.AlbumId);
            Assert.Equal(0, album.GenreId);
            Assert.Equal(0, album.ArtistId);
            Assert.Equal(string.Empty, album.Title);
            Assert.Equal(0m, album.Price);
            Assert.Equal(string.Empty, album.AlbumArtUrl);
            Assert.Null(album.Genre);
            Assert.Null(album.Artist);
            Assert.NotNull(album.OrderDetails);
            Assert.Empty(album.OrderDetails);
        }

        [Fact]
        public void Album_SetTitle_ReturnsCorrectTitle()
        {
            // Arrange
            var album = new Album();

            // Act
            album.Title = "Test Album";

            // Assert
            Assert.Equal("Test Album", album.Title);
        }

        [Fact]
        public void Album_SetPrice_ReturnsCorrectPrice()
        {
            // Arrange
            var album = new Album();

            // Act
            album.Price = 9.99m;

            // Assert
            Assert.Equal(9.99m, album.Price);
        }

        [Fact]
        public void Album_SetAlbumArtUrl_ReturnsCorrectUrl()
        {
            // Arrange
            var album = new Album();

            // Act
            album.AlbumArtUrl = "http://example.com/art.jpg";

            // Assert
            Assert.Equal("http://example.com/art.jpg", album.AlbumArtUrl);
        }

        [Fact]
        public void Album_SetGenreId_ReturnsCorrectGenreId()
        {
            // Arrange
            var album = new Album();

            // Act
            album.GenreId = 5;

            // Assert
            Assert.Equal(5, album.GenreId);
        }

        [Fact]
        public void Album_SetArtistId_ReturnsCorrectArtistId()
        {
            // Arrange
            var album = new Album();

            // Act
            album.ArtistId = 3;

            // Assert
            Assert.Equal(3, album.ArtistId);
        }

        [Fact]
        public void Album_SetGenre_ReturnsCorrectGenre()
        {
            // Arrange
            var album = new Album();
            var genre = new Genre { GenreId = 1, Name = "Rock" };

            // Act
            album.Genre = genre;

            // Assert
            Assert.NotNull(album.Genre);
            Assert.Equal("Rock", album.Genre.Name);
        }

        [Fact]
        public void Album_SetArtist_ReturnsCorrectArtist()
        {
            // Arrange
            var album = new Album();
            var artist = new Artist { ArtistId = 1, Name = "Test Artist" };

            // Act
            album.Artist = artist;

            // Assert
            Assert.NotNull(album.Artist);
            Assert.Equal("Test Artist", album.Artist.Name);
        }

        [Fact]
        public void Album_AddOrderDetail_OrderDetailsContainsItem()
        {
            // Arrange
            var album = new Album();
            var orderDetail = new OrderDetail { OrderDetailId = 1, Quantity = 2 };

            // Act
            album.OrderDetails.Add(orderDetail);

            // Assert
            Assert.Single(album.OrderDetails);
            Assert.Equal(1, album.OrderDetails[0].OrderDetailId);
        }

        [Fact]
        public void Album_SetAllProperties_AllPropertiesCorrect()
        {
            // Arrange & Act
            var album = new Album
            {
                AlbumId = 10,
                GenreId = 2,
                ArtistId = 3,
                Title = "Full Album",
                Price = 12.99m,
                AlbumArtUrl = "http://example.com/full.jpg"
            };

            // Assert
            Assert.Equal(10, album.AlbumId);
            Assert.Equal(2, album.GenreId);
            Assert.Equal(3, album.ArtistId);
            Assert.Equal("Full Album", album.Title);
            Assert.Equal(12.99m, album.Price);
            Assert.Equal("http://example.com/full.jpg", album.AlbumArtUrl);
        }
    }
}
