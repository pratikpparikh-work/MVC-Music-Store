using MvcMusicStore.Domain.Entities;
using Xunit;

namespace MvcMusicStore.Tests.Domain
{
    public class CartTests
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
            Assert.Equal(default(DateTime), cart.DateCreated);
            Assert.Null(cart.Album);
        }

        [Fact]
        public void Cart_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var dateCreated = DateTime.UtcNow;
            var album = new Album { AlbumId = 1, Title = "Test Album", Price = 9.99m };

            // Act
            var cart = new Cart
            {
                RecordId = 1,
                CartId = "test-cart-id",
                AlbumId = 1,
                Count = 2,
                DateCreated = dateCreated,
                Album = album
            };

            // Assert
            Assert.Equal(1, cart.RecordId);
            Assert.Equal("test-cart-id", cart.CartId);
            Assert.Equal(1, cart.AlbumId);
            Assert.Equal(2, cart.Count);
            Assert.Equal(dateCreated, cart.DateCreated);
            Assert.Equal(album, cart.Album);
        }

        [Fact]
        public void Cart_Count_CanBeIncremented()
        {
            // Arrange
            var cart = new Cart { RecordId = 1, CartId = "cart1", AlbumId = 1, Count = 1 };

            // Act
            cart.Count++;

            // Assert
            Assert.Equal(2, cart.Count);
        }

        [Fact]
        public void Cart_Count_CanBeDecremented()
        {
            // Arrange
            var cart = new Cart { RecordId = 1, CartId = "cart1", AlbumId = 1, Count = 3 };

            // Act
            cart.Count--;

            // Assert
            Assert.Equal(2, cart.Count);
        }

        [Fact]
        public void Cart_CartId_CanBeGuid()
        {
            // Arrange
            var guid = Guid.NewGuid().ToString();

            // Act
            var cart = new Cart { CartId = guid };

            // Assert
            Assert.Equal(guid, cart.CartId);
        }

        [Fact]
        public void Cart_CartId_CanBeUsername()
        {
            // Arrange & Act
            var cart = new Cart { CartId = "testuser@example.com" };

            // Assert
            Assert.Equal("testuser@example.com", cart.CartId);
        }

        [Fact]
        public void Cart_LineTotal_CalculatedCorrectly()
        {
            // Arrange
            var album = new Album { AlbumId = 1, Price = 9.99m };
            var cart = new Cart { Count = 3, Album = album };

            // Act
            var lineTotal = cart.Count * (cart.Album?.Price ?? 0);

            // Assert
            Assert.Equal(29.97m, lineTotal);
        }
    }
}
