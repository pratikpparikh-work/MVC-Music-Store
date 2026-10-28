using System;
using MvcMusicStore.Domain.Entities;
using Xunit;

namespace MvcMusicStore.Tests.Domain
{
    /// <summary>
    /// Unit tests for the Cart domain entity.
    /// </summary>
    public class CartEntityTests
    {
        [Fact]
        public void Cart_DefaultConstructor_InitializesProperties()
        {
            // Arrange & Act
            var cart = new Cart();

            // Assert
            Assert.Equal(0, cart.RecordId);
            Assert.Equal(string.Empty, cart.CartId);
            Assert.Equal(0, cart.AlbumId);
            Assert.Equal(0, cart.Count);
            Assert.Null(cart.Album);
        }

        [Fact]
        public void Cart_SetCartId_ReturnsCorrectCartId()
        {
            // Arrange
            var cart = new Cart();

            // Act
            cart.CartId = "test-cart-id";

            // Assert
            Assert.Equal("test-cart-id", cart.CartId);
        }

        [Fact]
        public void Cart_SetAlbumId_ReturnsCorrectAlbumId()
        {
            // Arrange
            var cart = new Cart();

            // Act
            cart.AlbumId = 5;

            // Assert
            Assert.Equal(5, cart.AlbumId);
        }

        [Fact]
        public void Cart_SetCount_ReturnsCorrectCount()
        {
            // Arrange
            var cart = new Cart();

            // Act
            cart.Count = 3;

            // Assert
            Assert.Equal(3, cart.Count);
        }

        [Fact]
        public void Cart_SetDateCreated_ReturnsCorrectDate()
        {
            // Arrange
            var cart = new Cart();
            var expectedDate = new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);

            // Act
            cart.DateCreated = expectedDate;

            // Assert
            Assert.Equal(expectedDate, cart.DateCreated);
        }

        [Fact]
        public void Cart_SetAlbum_ReturnsCorrectAlbum()
        {
            // Arrange
            var cart = new Cart();
            var album = new Album { AlbumId = 1, Title = "Test Album" };

            // Act
            cart.Album = album;

            // Assert
            Assert.NotNull(cart.Album);
            Assert.Equal("Test Album", cart.Album.Title);
        }

        [Fact]
        public void Cart_SetAllProperties_AllPropertiesCorrect()
        {
            // Arrange
            var now = DateTimeOffset.UtcNow;

            // Act
            var cart = new Cart
            {
                RecordId = 1,
                CartId = "cart-abc",
                AlbumId = 10,
                Count = 2,
                DateCreated = now
            };

            // Assert
            Assert.Equal(1, cart.RecordId);
            Assert.Equal("cart-abc", cart.CartId);
            Assert.Equal(10, cart.AlbumId);
            Assert.Equal(2, cart.Count);
            Assert.Equal(now, cart.DateCreated);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(100)]
        public void Cart_SetVariousCounts_ReturnsCorrectCount(int count)
        {
            // Arrange
            var cart = new Cart();

            // Act
            cart.Count = count;

            // Assert
            Assert.Equal(count, cart.Count);
        }
    }
}
