using System;
using System.Collections.Generic;
using Xunit;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Models
{
    public class AlbumTests
    {
        [Fact]
        public void Album_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var album = new Album();

            // Assert
            Assert.Equal(0, album.AlbumId);
            Assert.Equal(0, album.GenreId);
            Assert.Equal(0, album.ArtistId);
            Assert.Equal(string.Empty, album.Title);
            Assert.Equal(0m, album.Price);
            Assert.Null(album.AlbumArtUrl);
            Assert.Null(album.Genre);
            Assert.Null(album.Artist);
            Assert.NotNull(album.OrderDetails);
            Assert.Empty(album.OrderDetails);
        }

        [Fact]
        public void Album_SetAlbumId_ReturnsCorrectValue()
        {
            var album = new Album();
            album.AlbumId = 42;
            Assert.Equal(42, album.AlbumId);
        }

        [Fact]
        public void Album_SetTitle_ReturnsCorrectValue()
        {
            var album = new Album();
            album.Title = "Nevermind";
            Assert.Equal("Nevermind", album.Title);
        }

        [Fact]
        public void Album_SetPrice_ReturnsCorrectValue()
        {
            var album = new Album();
            album.Price = 9.99m;
            Assert.Equal(9.99m, album.Price);
        }

        [Fact]
        public void Album_SetAlbumArtUrl_ReturnsCorrectValue()
        {
            var album = new Album();
            album.AlbumArtUrl = "/images/nevermind.jpg";
            Assert.Equal("/images/nevermind.jpg", album.AlbumArtUrl);
        }

        [Fact]
        public void Album_SetAlbumArtUrlToNull_ReturnsNull()
        {
            var album = new Album { AlbumArtUrl = "/images/test.jpg" };
            album.AlbumArtUrl = null;
            Assert.Null(album.AlbumArtUrl);
        }

        [Fact]
        public void Album_SetGenre_ReturnsCorrectGenre()
        {
            var album = new Album();
            var genre = new Genre { GenreId = 1, Name = "Rock" };
            album.Genre = genre;
            Assert.NotNull(album.Genre);
            Assert.Equal("Rock", album.Genre.Name);
        }

        [Fact]
        public void Album_SetArtist_ReturnsCorrectArtist()
        {
            var album = new Album();
            var artist = new Artist { ArtistId = 1, Name = "Nirvana" };
            album.Artist = artist;
            Assert.NotNull(album.Artist);
            Assert.Equal("Nirvana", album.Artist.Name);
        }

        [Fact]
        public void Album_AddOrderDetail_OrderDetailIsInList()
        {
            var album = new Album { AlbumId = 1, Title = "Test Album" };
            var orderDetail = new OrderDetail { OrderDetailId = 1, AlbumId = 1 };
            album.OrderDetails.Add(orderDetail);
            Assert.Single(album.OrderDetails);
        }

        [Fact]
        public void Album_InitializeWithAllValues_PropertiesAreSet()
        {
            var genre = new Genre { GenreId = 1, Name = "Rock" };
            var artist = new Artist { ArtistId = 1, Name = "Nirvana" };
            var album = new Album
            {
                AlbumId = 1,
                GenreId = 1,
                ArtistId = 1,
                Title = "Nevermind",
                Price = 8.99m,
                AlbumArtUrl = "/images/nevermind.jpg",
                Genre = genre,
                Artist = artist
            };

            Assert.Equal(1, album.AlbumId);
            Assert.Equal(1, album.GenreId);
            Assert.Equal(1, album.ArtistId);
            Assert.Equal("Nevermind", album.Title);
            Assert.Equal(8.99m, album.Price);
            Assert.Equal("/images/nevermind.jpg", album.AlbumArtUrl);
            Assert.Equal("Rock", album.Genre.Name);
            Assert.Equal("Nirvana", album.Artist.Name);
        }

        [Fact]
        public void Album_PriceAtMinimumBoundary_IsValid()
        {
            var album = new Album();
            album.Price = 0.01m;
            Assert.Equal(0.01m, album.Price);
        }

        [Fact]
        public void Album_PriceAtMaximumBoundary_IsValid()
        {
            var album = new Album();
            album.Price = 100.00m;
            Assert.Equal(100.00m, album.Price);
        }

        [Fact]
        public void Album_SetGenreId_ReturnsCorrectValue()
        {
            var album = new Album();
            album.GenreId = 5;
            Assert.Equal(5, album.GenreId);
        }

        [Fact]
        public void Album_SetArtistId_ReturnsCorrectValue()
        {
            var album = new Album();
            album.ArtistId = 10;
            Assert.Equal(10, album.ArtistId);
        }

        [Fact]
        public void Album_OrderDetails_CanHoldMultipleItems()
        {
            var album = new Album { AlbumId = 1 };
            album.OrderDetails.Add(new OrderDetail { OrderDetailId = 1 });
            album.OrderDetails.Add(new OrderDetail { OrderDetailId = 2 });
            album.OrderDetails.Add(new OrderDetail { OrderDetailId = 3 });
            Assert.Equal(3, album.OrderDetails.Count);
        }

        [Fact]
        public void Album_SetTitleToEmpty_ReturnsEmptyString()
        {
            var album = new Album { Title = "Some Title" };
            album.Title = string.Empty;
            Assert.Equal(string.Empty, album.Title);
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(50.00)]
        [InlineData(100.00)]
        public void Album_SetVariousPrices_ReturnsCorrectValue(double price)
        {
            var album = new Album();
            album.Price = (decimal)price;
            Assert.Equal((decimal)price, album.Price);
        }
    }
}
