using Moq;
using MvcMusicStore.Application.Services;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Domain.Interfaces;
using Xunit;

namespace MvcMusicStore.Tests.Application
{
    public class ShoppingCartServiceTests
    {
        private readonly Mock<ICartRepository> _cartRepoMock;
        private readonly Mock<IAlbumRepository> _albumRepoMock;
        private readonly Mock<IOrderDetailRepository> _orderDetailRepoMock;
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly ShoppingCartService _service;

        public ShoppingCartServiceTests()
        {
            _cartRepoMock = new Mock<ICartRepository>();
            _albumRepoMock = new Mock<IAlbumRepository>();
            _orderDetailRepoMock = new Mock<IOrderDetailRepository>();
            _orderRepoMock = new Mock<IOrderRepository>();
            _service = new ShoppingCartService(
                _cartRepoMock.Object,
                _albumRepoMock.Object,
                _orderDetailRepoMock.Object,
                _orderRepoMock.Object);
        }

        [Fact]
        public async Task GetCartAsync_ReturnsShoppingCartDto()
        {
            // Arrange
            var cartId = "test-cart";
            var cartItems = new List<Cart>
            {
                new Cart
                {
                    RecordId = 1, CartId = cartId, AlbumId = 1, Count = 2,
                    DateCreated = DateTime.UtcNow,
                    Album = new Album { AlbumId = 1, Title = "Test Album", Price = 9.99m, AlbumArtUrl = "/img/1.jpg" }
                }
            };
            _cartRepoMock.Setup(r => r.GetCartItemsAsync(cartId)).ReturnsAsync(cartItems);
            _cartRepoMock.Setup(r => r.GetCartTotalAsync(cartId)).ReturnsAsync(19.98m);

            // Act
            var result = await _service.GetCartAsync(cartId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.CartItems);
            Assert.Equal(19.98m, result.CartTotal);
            Assert.Equal("Test Album", result.CartItems[0].AlbumTitle);
            Assert.Equal(9.99m, result.CartItems[0].AlbumPrice);
        }

        [Fact]
        public async Task GetCartAsync_EmptyCart_ReturnsEmptyDto()
        {
            // Arrange
            var cartId = "empty-cart";
            _cartRepoMock.Setup(r => r.GetCartItemsAsync(cartId)).ReturnsAsync(new List<Cart>());
            _cartRepoMock.Setup(r => r.GetCartTotalAsync(cartId)).ReturnsAsync(0m);

            // Act
            var result = await _service.GetCartAsync(cartId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.CartItems);
            Assert.Equal(0m, result.CartTotal);
        }

        [Fact]
        public async Task AddToCartAsync_NewItem_AddsCartItem()
        {
            // Arrange
            var cartId = "test-cart";
            _cartRepoMock.Setup(r => r.GetCartItemAsync(cartId, 1)).ReturnsAsync((Cart?)null);
            _cartRepoMock.Setup(r => r.AddAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);
            _cartRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _service.AddToCartAsync(cartId, 1);

            // Assert
            _cartRepoMock.Verify(r => r.AddAsync(It.Is<Cart>(c => c.CartId == cartId && c.AlbumId == 1 && c.Count == 1)), Times.Once);
            _cartRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddToCartAsync_ExistingItem_IncrementsCount()
        {
            // Arrange
            var cartId = "test-cart";
            var existingCart = new Cart { RecordId = 1, CartId = cartId, AlbumId = 1, Count = 2 };
            _cartRepoMock.Setup(r => r.GetCartItemAsync(cartId, 1)).ReturnsAsync(existingCart);
            _cartRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _service.AddToCartAsync(cartId, 1);

            // Assert
            Assert.Equal(3, existingCart.Count);
            _cartRepoMock.Verify(r => r.Update(existingCart), Times.Once);
            _cartRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveFromCartAsync_ItemCountGreaterThanOne_DecrementsCount()
        {
            // Arrange
            var cartId = "test-cart";
            var cartItem = new Cart { RecordId = 1, CartId = cartId, AlbumId = 1, Count = 3 };
            _cartRepoMock.Setup(r => r.GetCartItemByRecordIdAsync(cartId, 1)).ReturnsAsync(cartItem);
            _cartRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.RemoveFromCartAsync(cartId, 1);

            // Assert
            Assert.Equal(2, result);
            Assert.Equal(2, cartItem.Count);
            _cartRepoMock.Verify(r => r.Update(cartItem), Times.Once);
        }

        [Fact]
        public async Task RemoveFromCartAsync_ItemCountOne_RemovesItem()
        {
            // Arrange
            var cartId = "test-cart";
            var cartItem = new Cart { RecordId = 1, CartId = cartId, AlbumId = 1, Count = 1 };
            _cartRepoMock.Setup(r => r.GetCartItemByRecordIdAsync(cartId, 1)).ReturnsAsync(cartItem);
            _cartRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.RemoveFromCartAsync(cartId, 1);

            // Assert
            Assert.Equal(0, result);
            _cartRepoMock.Verify(r => r.Remove(cartItem), Times.Once);
        }

        [Fact]
        public async Task RemoveFromCartAsync_ItemNotFound_ReturnsZero()
        {
            // Arrange
            var cartId = "test-cart";
            _cartRepoMock.Setup(r => r.GetCartItemByRecordIdAsync(cartId, 999)).ReturnsAsync((Cart?)null);

            // Act
            var result = await _service.RemoveFromCartAsync(cartId, 999);

            // Assert
            Assert.Equal(0, result);
            _cartRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task GetCartCountAsync_ReturnsCount()
        {
            // Arrange
            var cartId = "test-cart";
            _cartRepoMock.Setup(r => r.GetCartCountAsync(cartId)).ReturnsAsync(5);

            // Act
            var result = await _service.GetCartCountAsync(cartId);

            // Assert
            Assert.Equal(5, result);
        }

        [Fact]
        public async Task GetCartTotalAsync_ReturnsTotal()
        {
            // Arrange
            var cartId = "test-cart";
            _cartRepoMock.Setup(r => r.GetCartTotalAsync(cartId)).ReturnsAsync(49.95m);

            // Act
            var result = await _service.GetCartTotalAsync(cartId);

            // Assert
            Assert.Equal(49.95m, result);
        }

        [Fact]
        public async Task MigrateCartAsync_CallsRepository()
        {
            // Arrange
            var oldCartId = "old-cart";
            var newCartId = "new-cart";
            _cartRepoMock.Setup(r => r.MigrateCartAsync(oldCartId, newCartId)).Returns(Task.CompletedTask);

            // Act
            await _service.MigrateCartAsync(oldCartId, newCartId);

            // Assert
            _cartRepoMock.Verify(r => r.MigrateCartAsync(oldCartId, newCartId), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_WithCartItems_CreatesOrderAndDetails()
        {
            // Arrange
            var cartId = "test-cart";
            var order = new Order { Username = "testuser", OrderDate = DateTime.UtcNow };
            var cartItems = new List<Cart>
            {
                new Cart { RecordId = 1, CartId = cartId, AlbumId = 1, Count = 2,
                    Album = new Album { AlbumId = 1, Price = 9.99m } },
                new Cart { RecordId = 2, CartId = cartId, AlbumId = 2, Count = 1,
                    Album = new Album { AlbumId = 2, Price = 14.99m } }
            };
            _cartRepoMock.Setup(r => r.GetCartItemsAsync(cartId)).ReturnsAsync(cartItems);
            _orderRepoMock.Setup(r => r.AddAsync(order)).Returns(Task.CompletedTask);
            _orderRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            _orderDetailRepoMock.Setup(r => r.AddAsync(It.IsAny<OrderDetail>())).Returns(Task.CompletedTask);
            _cartRepoMock.Setup(r => r.EmptyCartAsync(cartId)).Returns(Task.CompletedTask);

            // Act
            var orderId = await _service.CreateOrderAsync(cartId, order);

            // Assert
            _orderRepoMock.Verify(r => r.AddAsync(order), Times.Once);
            _orderDetailRepoMock.Verify(r => r.AddAsync(It.IsAny<OrderDetail>()), Times.Exactly(2));
            _cartRepoMock.Verify(r => r.EmptyCartAsync(cartId), Times.Once);
            Assert.Equal(34.97m, order.Total);
        }

        [Fact]
        public async Task GetAlbumTitleByCartRecordIdAsync_ExistingRecord_ReturnsTitle()
        {
            // Arrange
            var cartId = "test-cart";
            var cartItem = new Cart
            {
                RecordId = 1, CartId = cartId, AlbumId = 1,
                Album = new Album { AlbumId = 1, Title = "Test Album" }
            };
            _cartRepoMock.Setup(r => r.GetCartItemByRecordIdAsync(cartId, 1)).ReturnsAsync(cartItem);

            // Act
            var result = await _service.GetAlbumTitleByCartRecordIdAsync(cartId, 1);

            // Assert
            Assert.Equal("Test Album", result);
        }

        [Fact]
        public async Task GetAlbumTitleByCartRecordIdAsync_NonExistingRecord_ReturnsNull()
        {
            // Arrange
            var cartId = "test-cart";
            _cartRepoMock.Setup(r => r.GetCartItemByRecordIdAsync(cartId, 999)).ReturnsAsync((Cart?)null);

            // Act
            var result = await _service.GetAlbumTitleByCartRecordIdAsync(cartId, 999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCartAsync_CartItemWithNullAlbum_HandlesGracefully()
        {
            // Arrange
            var cartId = "test-cart";
            var cartItems = new List<Cart>
            {
                new Cart { RecordId = 1, CartId = cartId, AlbumId = 1, Count = 1, Album = null }
            };
            _cartRepoMock.Setup(r => r.GetCartItemsAsync(cartId)).ReturnsAsync(cartItems);
            _cartRepoMock.Setup(r => r.GetCartTotalAsync(cartId)).ReturnsAsync(0m);

            // Act
            var result = await _service.GetCartAsync(cartId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.CartItems);
            Assert.Equal(string.Empty, result.CartItems[0].AlbumTitle);
            Assert.Equal(0m, result.CartItems[0].AlbumPrice);
        }
    }
}
