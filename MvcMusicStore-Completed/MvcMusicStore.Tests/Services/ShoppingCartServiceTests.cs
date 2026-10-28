using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Application.Services;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Domain.Interfaces;
using Xunit;

namespace MvcMusicStore.Tests.Services
{
    /// <summary>
    /// Unit tests for ShoppingCartService.
    /// </summary>
    public class ShoppingCartServiceTests
    {
        private readonly Mock<ICartRepository> _mockCartRepository;
        private readonly Mock<IAlbumRepository> _mockAlbumRepository;
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly IShoppingCartService _cartService;

        public ShoppingCartServiceTests()
        {
            _mockCartRepository = new Mock<ICartRepository>();
            _mockAlbumRepository = new Mock<IAlbumRepository>();
            _mockOrderRepository = new Mock<IOrderRepository>();
            _cartService = new ShoppingCartService(
                _mockCartRepository.Object,
                _mockAlbumRepository.Object,
                _mockOrderRepository.Object);
        }

        [Fact]
        public async Task GetCartItemsAsync_ReturnsCartItems()
        {
            // Arrange
            var cartId = "test-cart";
            var expectedItems = new List<Cart>
            {
                new Cart { RecordId = 1, CartId = cartId, AlbumId = 1, Count = 2 },
                new Cart { RecordId = 2, CartId = cartId, AlbumId = 2, Count = 1 }
            };
            _mockCartRepository.Setup(r => r.GetCartItemsAsync(cartId))
                .ReturnsAsync(expectedItems);

            // Act
            var result = await _cartService.GetCartItemsAsync(cartId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task AddToCartAsync_WhenItemNotInCart_AddsNewItem()
        {
            // Arrange
            var cartId = "test-cart";
            var albumId = 1;
            _mockCartRepository.Setup(r => r.GetCartItemAsync(cartId, albumId))
                .ReturnsAsync((Cart?)null);
            _mockCartRepository.Setup(r => r.AddCartItemAsync(It.IsAny<Cart>()))
                .Returns(Task.CompletedTask);

            // Act
            await _cartService.AddToCartAsync(cartId, albumId);

            // Assert
            _mockCartRepository.Verify(r => r.AddCartItemAsync(It.Is<Cart>(c =>
                c.CartId == cartId && c.AlbumId == albumId && c.Count == 1)), Times.Once);
        }

        [Fact]
        public async Task AddToCartAsync_WhenItemAlreadyInCart_IncrementsCount()
        {
            // Arrange
            var cartId = "test-cart";
            var albumId = 1;
            var existingItem = new Cart { RecordId = 1, CartId = cartId, AlbumId = albumId, Count = 2 };
            _mockCartRepository.Setup(r => r.GetCartItemAsync(cartId, albumId))
                .ReturnsAsync(existingItem);
            _mockCartRepository.Setup(r => r.UpdateCartItemAsync(It.IsAny<Cart>()))
                .Returns(Task.CompletedTask);

            // Act
            await _cartService.AddToCartAsync(cartId, albumId);

            // Assert
            _mockCartRepository.Verify(r => r.UpdateCartItemAsync(It.Is<Cart>(c => c.Count == 3)), Times.Once);
        }

        [Fact]
        public async Task RemoveFromCartAsync_WhenItemCountGreaterThanOne_DecrementsCount()
        {
            // Arrange
            var cartId = "test-cart";
            var recordId = 1;
            var cartItem = new Cart { RecordId = recordId, CartId = cartId, AlbumId = 1, Count = 3 };
            _mockCartRepository.Setup(r => r.GetCartItemByRecordIdAsync(recordId))
                .ReturnsAsync(cartItem);
            _mockCartRepository.Setup(r => r.UpdateCartItemAsync(It.IsAny<Cart>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _cartService.RemoveFromCartAsync(cartId, recordId);

            // Assert
            Assert.Equal(2, result);
            _mockCartRepository.Verify(r => r.UpdateCartItemAsync(It.Is<Cart>(c => c.Count == 2)), Times.Once);
        }

        [Fact]
        public async Task RemoveFromCartAsync_WhenItemCountIsOne_RemovesItem()
        {
            // Arrange
            var cartId = "test-cart";
            var recordId = 1;
            var cartItem = new Cart { RecordId = recordId, CartId = cartId, AlbumId = 1, Count = 1 };
            _mockCartRepository.Setup(r => r.GetCartItemByRecordIdAsync(recordId))
                .ReturnsAsync(cartItem);
            _mockCartRepository.Setup(r => r.RemoveCartItemAsync(It.IsAny<Cart>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _cartService.RemoveFromCartAsync(cartId, recordId);

            // Assert
            Assert.Equal(0, result);
            _mockCartRepository.Verify(r => r.RemoveCartItemAsync(cartItem), Times.Once);
        }

        [Fact]
        public async Task RemoveFromCartAsync_WhenItemNotFound_ReturnsZero()
        {
            // Arrange
            var cartId = "test-cart";
            var recordId = 999;
            _mockCartRepository.Setup(r => r.GetCartItemByRecordIdAsync(recordId))
                .ReturnsAsync((Cart?)null);

            // Act
            var result = await _cartService.RemoveFromCartAsync(cartId, recordId);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task RemoveFromCartAsync_WhenItemBelongsToDifferentCart_ReturnsZero()
        {
            // Arrange
            var cartId = "test-cart";
            var recordId = 1;
            var cartItem = new Cart { RecordId = recordId, CartId = "other-cart", AlbumId = 1, Count = 2 };
            _mockCartRepository.Setup(r => r.GetCartItemByRecordIdAsync(recordId))
                .ReturnsAsync(cartItem);

            // Act
            var result = await _cartService.RemoveFromCartAsync(cartId, recordId);

            // Assert
            Assert.Equal(0, result);
            _mockCartRepository.Verify(r => r.UpdateCartItemAsync(It.IsAny<Cart>()), Times.Never);
            _mockCartRepository.Verify(r => r.RemoveCartItemAsync(It.IsAny<Cart>()), Times.Never);
        }

        [Fact]
        public async Task GetCartCountAsync_ReturnsCorrectCount()
        {
            // Arrange
            var cartId = "test-cart";
            _mockCartRepository.Setup(r => r.GetCartCountAsync(cartId))
                .ReturnsAsync(5);

            // Act
            var result = await _cartService.GetCartCountAsync(cartId);

            // Assert
            Assert.Equal(5, result);
        }

        [Fact]
        public async Task GetCartTotalAsync_ReturnsCorrectTotal()
        {
            // Arrange
            var cartId = "test-cart";
            _mockCartRepository.Setup(r => r.GetCartTotalAsync(cartId))
                .ReturnsAsync(49.95m);

            // Act
            var result = await _cartService.GetCartTotalAsync(cartId);

            // Assert
            Assert.Equal(49.95m, result);
        }

        [Fact]
        public async Task MigrateCartAsync_CallsRepositoryMigrate()
        {
            // Arrange
            var oldCartId = "old-cart";
            var newCartId = "new-cart";
            _mockCartRepository.Setup(r => r.MigrateCartAsync(oldCartId, newCartId))
                .Returns(Task.CompletedTask);

            // Act
            await _cartService.MigrateCartAsync(oldCartId, newCartId);

            // Assert
            _mockCartRepository.Verify(r => r.MigrateCartAsync(oldCartId, newCartId), Times.Once);
        }

        [Fact]
        public async Task EmptyCartAsync_CallsRepositoryEmpty()
        {
            // Arrange
            var cartId = "test-cart";
            _mockCartRepository.Setup(r => r.EmptyCartAsync(cartId))
                .Returns(Task.CompletedTask);

            // Act
            await _cartService.EmptyCartAsync(cartId);

            // Assert
            _mockCartRepository.Verify(r => r.EmptyCartAsync(cartId), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_WithCartItems_CalculatesOrderTotal()
        {
            // Arrange
            var cartId = "test-cart";
            var order = new Order { OrderId = 1, Username = "testuser" };
            var cartItems = new List<Cart>
            {
                new Cart { RecordId = 1, CartId = cartId, AlbumId = 1, Count = 2 },
                new Cart { RecordId = 2, CartId = cartId, AlbumId = 2, Count = 1 }
            };
            var album1 = new Album { AlbumId = 1, Price = 9.99m };
            var album2 = new Album { AlbumId = 2, Price = 14.99m };

            _mockCartRepository.Setup(r => r.GetCartItemsAsync(cartId))
                .ReturnsAsync(cartItems);
            _mockAlbumRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(album1);
            _mockAlbumRepository.Setup(r => r.GetByIdAsync(2))
                .ReturnsAsync(album2);
            _mockOrderRepository.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);
            _mockCartRepository.Setup(r => r.EmptyCartAsync(cartId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _cartService.CreateOrderAsync(cartId, order);

            // Assert
            Assert.Equal(1, result); // OrderId
            Assert.Equal(34.97m, order.Total); // 2*9.99 + 1*14.99
        }

        [Fact]
        public async Task CreateOrderAsync_WithEmptyCart_SetsZeroTotal()
        {
            // Arrange
            var cartId = "empty-cart";
            var order = new Order { OrderId = 2, Username = "testuser" };

            _mockCartRepository.Setup(r => r.GetCartItemsAsync(cartId))
                .ReturnsAsync(new List<Cart>());
            _mockOrderRepository.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);
            _mockCartRepository.Setup(r => r.EmptyCartAsync(cartId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _cartService.CreateOrderAsync(cartId, order);

            // Assert
            Assert.Equal(2, result);
            Assert.Equal(0m, order.Total);
        }

        [Fact]
        public async Task CreateOrderAsync_EmptiesCartAfterCreation()
        {
            // Arrange
            var cartId = "test-cart";
            var order = new Order { OrderId = 1 };

            _mockCartRepository.Setup(r => r.GetCartItemsAsync(cartId))
                .ReturnsAsync(new List<Cart>());
            _mockOrderRepository.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);
            _mockCartRepository.Setup(r => r.EmptyCartAsync(cartId))
                .Returns(Task.CompletedTask);

            // Act
            await _cartService.CreateOrderAsync(cartId, order);

            // Assert
            _mockCartRepository.Verify(r => r.EmptyCartAsync(cartId), Times.Once);
        }

        [Fact]
        public async Task AddToCartAsync_NewItem_SetsDateCreated()
        {
            // Arrange
            var cartId = "test-cart";
            var albumId = 5;
            Cart? capturedCart = null;

            _mockCartRepository.Setup(r => r.GetCartItemAsync(cartId, albumId))
                .ReturnsAsync((Cart?)null);
            _mockCartRepository.Setup(r => r.AddCartItemAsync(It.IsAny<Cart>()))
                .Callback<Cart>(c => capturedCart = c)
                .Returns(Task.CompletedTask);

            // Act
            await _cartService.AddToCartAsync(cartId, albumId);

            // Assert
            Assert.NotNull(capturedCart);
            Assert.True(capturedCart.DateCreated > DateTimeOffset.MinValue);
        }
    }
}
