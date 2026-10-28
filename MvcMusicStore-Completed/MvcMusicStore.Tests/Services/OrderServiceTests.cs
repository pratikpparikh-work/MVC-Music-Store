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
    /// Unit tests for OrderService.
    /// </summary>
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly IOrderService _orderService;

        public OrderServiceTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _orderService = new OrderService(_mockOrderRepository.Object);
        }

        [Fact]
        public async Task GetOrderByIdAsync_WithValidId_ReturnsOrder()
        {
            // Arrange
            var expectedOrder = new Order
            {
                OrderId = 1,
                Username = "testuser",
                FirstName = "John",
                LastName = "Doe",
                Total = 29.99m
            };
            _mockOrderRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(expectedOrder);

            // Act
            var result = await _orderService.GetOrderByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.OrderId);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task GetOrderByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            _mockOrderRepository.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Order?)null);

            // Act
            var result = await _orderService.GetOrderByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrderByIdAsync_CallsRepositoryOnce()
        {
            // Arrange
            _mockOrderRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Order { OrderId = 1 });

            // Act
            await _orderService.GetOrderByIdAsync(1);

            // Assert
            _mockOrderRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task IsOrderOwnedByUserAsync_WhenOrderBelongsToUser_ReturnsTrue()
        {
            // Arrange
            _mockOrderRepository.Setup(r => r.IsOrderOwnedByUserAsync(1, "testuser"))
                .ReturnsAsync(true);

            // Act
            var result = await _orderService.IsOrderOwnedByUserAsync(1, "testuser");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsOrderOwnedByUserAsync_WhenOrderDoesNotBelongToUser_ReturnsFalse()
        {
            // Arrange
            _mockOrderRepository.Setup(r => r.IsOrderOwnedByUserAsync(1, "otheruser"))
                .ReturnsAsync(false);

            // Act
            var result = await _orderService.IsOrderOwnedByUserAsync(1, "otheruser");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsOrderOwnedByUserAsync_CallsRepositoryWithCorrectParameters()
        {
            // Arrange
            _mockOrderRepository.Setup(r => r.IsOrderOwnedByUserAsync(5, "admin"))
                .ReturnsAsync(true);

            // Act
            await _orderService.IsOrderOwnedByUserAsync(5, "admin");

            // Assert
            _mockOrderRepository.Verify(r => r.IsOrderOwnedByUserAsync(5, "admin"), Times.Once);
        }

        [Theory]
        [InlineData(1, "user1", true)]
        [InlineData(2, "user2", false)]
        [InlineData(3, "admin", true)]
        public async Task IsOrderOwnedByUserAsync_WithVariousInputs_ReturnsExpectedResult(
            int orderId, string username, bool expected)
        {
            // Arrange
            _mockOrderRepository.Setup(r => r.IsOrderOwnedByUserAsync(orderId, username))
                .ReturnsAsync(expected);

            // Act
            var result = await _orderService.IsOrderOwnedByUserAsync(orderId, username);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
