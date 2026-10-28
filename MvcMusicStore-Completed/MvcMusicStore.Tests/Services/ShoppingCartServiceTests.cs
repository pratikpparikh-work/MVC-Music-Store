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
            var cartId = "test-cart-id";
            var cartItems = new List<Cart>
            {
                new Cart { RecordId = 1, CartId = cartId, AlbumId = 1, Count = 2 }
            };
            _mockCartRepository.Setup(r => r.GetCartItemsAsync(cartId))
                .ReturnsAsync(cartItems);

            // Act
            var result = await _cartService.GetCartItemsAsync(cartId);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task AddToCartAsync_NewItem_CreatesCartItem()
        {
            // Arrange
            var cartId = "test-cart-id";
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
        public async Task AddToCartAsync_ExistingItem_IncrementsCount()
        {
            // Arrange
            var cartId = "test-cart-id";
            var albumId = 1;
            var existingItem = new Cart { RecordId = 1, CartId = cartId, AlbumId = albumId, Count = 1 };
            _mockCartRepository.Setup(r => r.GetCartItemAsync(cartId, albumId))
                .ReturnsAsync(existingItem);
            _mockCartRepository.Setup(r => r.UpdateCartItemAsync(It.IsAny<Cart>()))
                .Returns(Task.CompletedTask);

            // Act
            await _cartService.AddToCartAsync(cartId, albumId);

            // Assert
            _mockCartRepository.Verify(r => r.UpdateCartItemAsync(It.Is<Cart>(c =>
                c.Count == 2)), Times.Once);
        }

        [Fact]
        public async Task RemoveFromCartAsync_SingleItem_RemovesItem()
        {
            // Arrange
            var cartId = "test-cart-id";
            var recordId = 1;
            var cartItem = new Cart { RecordId = recordId, CartId = cartId, AlbumId = 1, Count = 1 };
            _mockCartRepository.Setup(r => r.GetCartItemByRecordIdAsync(recordId))
                .ReturnsAsync(cartItem);
            _mockCartRepository.Setup(r => r.RemoveCartItemAsync(cartItem))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _cartService.RemoveFromCartAsync(cartId, recordId);

            // Assert
            Assert.Equal(0, result);
            _mockCartRepository.Verify(r => r.RemoveCartItemAsync(cartItem), Times.Once);
        }

        [Fact]
        public async Task GetCartCountAsync_ReturnsCount()
        {
            // Arrange
            var cartId = "test-cart-id";
            _mockCartRepository.Setup(r => r.GetCartCountAsync(cartId))
                .ReturnsAsync(5);

            // Act
            var result = await _cartService.GetCartCountAsync(cartId);

            // Assert
            Assert.Equal(5, result);
        }

        [Fact]
        public async Task GetCartTotalAsync_ReturnsTotal()
        {
            // Arrange
            var cartId = "test-cart-id";
            _mockCartRepository.Setup(r => r.GetCartTotalAsync(cartId))
                .ReturnsAsync(49.95M);

            // Act
            var result = await _cartService.GetCartTotalAsync(cartId);

            // Assert
            Assert.Equal(49.95M, result);
        }
    }
}
