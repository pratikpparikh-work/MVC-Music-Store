using System;
using Xunit;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Models
{
    public class CartTests
    {
        [Fact]
        public void Cart_DefaultConstructor_SetsDefaultValues()
        {
            var cart = new Cart();
            Assert.Equal(0, cart.RecordId);
            Assert.Equal(string.Empty, cart.CartId);
            Assert.Equal(0, cart.AlbumId);
            Assert.Equal(0, cart.Count);
            Assert.Null(cart.Album);
        }

        [Fact]
        public void Cart_SetRecordId_ReturnsCorrectValue()
        {
            var cart = new Cart();
            cart.RecordId = 5;
            Assert.Equal(5, cart.RecordId);
        }

        [Fact]
        public void Cart_SetCartId_ReturnsCorrectValue()
        {
            var cart = new Cart();
            cart.CartId = "test-cart-id";
            Assert.Equal("test-cart-id", cart.CartId);
        }

        [Fact]
        public void Cart_SetAlbumId_ReturnsCorrectValue()
        {
            var cart = new Cart();
            cart.AlbumId = 10;
            Assert.Equal(10, cart.AlbumId);
        }

        [Fact]
        public void Cart_SetCount_ReturnsCorrectValue()
        {
            var cart = new Cart();
            cart.Count = 3;
            Assert.Equal(3, cart.Count);
        }

        [Fact]
        public void Cart_SetDateCreated_ReturnsCorrectValue()
        {
            var cart = new Cart();
            var date = new DateTime(2024, 6, 15);
            cart.DateCreated = date;
            Assert.Equal(date, cart.DateCreated);
        }

        [Fact]
        public void Cart_SetAlbum_ReturnsCorrectAlbum()
        {
            var cart = new Cart();
            var album = new Album { AlbumId = 1, Title = "Test Album" };
            cart.Album = album;
            Assert.NotNull(cart.Album);
            Assert.Equal("Test Album", cart.Album.Title);
        }

        [Fact]
        public void Cart_SetAlbumToNull_ReturnsNull()
        {
            var cart = new Cart { Album = new Album { AlbumId = 1 } };
            cart.Album = null;
            Assert.Null(cart.Album);
        }

        [Fact]
        public void Cart_InitializeWithAllValues_PropertiesAreSet()
        {
            var album = new Album { AlbumId = 1, Title = "Nevermind" };
            var date = new DateTime(2024, 1, 1);
            var cart = new Cart
            {
                RecordId = 1,
                CartId = "user-cart",
                AlbumId = 1,
                Count = 2,
                DateCreated = date,
                Album = album
            };

            Assert.Equal(1, cart.RecordId);
            Assert.Equal("user-cart", cart.CartId);
            Assert.Equal(1, cart.AlbumId);
            Assert.Equal(2, cart.Count);
            Assert.Equal(date, cart.DateCreated);
            Assert.Equal("Nevermind", cart.Album.Title);
        }

        [Fact]
        public void Cart_SetCartIdToGuid_ReturnsCorrectValue()
        {
            var cart = new Cart();
            var guid = Guid.NewGuid().ToString();
            cart.CartId = guid;
            Assert.Equal(guid, cart.CartId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void Cart_SetVariousCounts_ReturnsCorrectValue(int count)
        {
            var cart = new Cart();
            cart.Count = count;
            Assert.Equal(count, cart.Count);
        }
    }
}
