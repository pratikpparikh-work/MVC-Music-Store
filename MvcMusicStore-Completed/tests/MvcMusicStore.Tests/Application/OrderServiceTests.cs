using Moq;
using MvcMusicStore.Application.Services;
using MvcMusicStore.Domain.Interfaces;
using Xunit;

namespace MvcMusicStore.Tests.Application
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _orderRepoMock = new Mock<IOrderRepository>();
            _service = new OrderService(_orderRepoMock.Object);
        }

        [Fact]
        public async Task ValidateOrderOwnerAsync_ValidOwner_ReturnsTrue()
        {
            // Arrange
            _orderRepoMock.Setup(r => r.ValidateOrderOwnerAsync(1, "testuser")).ReturnsAsync(true);

            // Act
            var result = await _service.ValidateOrderOwnerAsync(1, "testuser");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidateOrderOwnerAsync_InvalidOwner_ReturnsFalse()
        {
            // Arrange
            _orderRepoMock.Setup(r => r.ValidateOrderOwnerAsync(1, "wronguser")).ReturnsAsync(false);

            // Act
            var result = await _service.ValidateOrderOwnerAsync(1, "wronguser");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateOrderOwnerAsync_NonExistingOrder_ReturnsFalse()
        {
            // Arrange
            _orderRepoMock.Setup(r => r.ValidateOrderOwnerAsync(999, "testuser")).ReturnsAsync(false);

            // Act
            var result = await _service.ValidateOrderOwnerAsync(999, "testuser");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateOrderOwnerAsync_EmptyUsername_ReturnsFalse()
        {
            // Arrange
            _orderRepoMock.Setup(r => r.ValidateOrderOwnerAsync(1, string.Empty)).ReturnsAsync(false);

            // Act
            var result = await _service.ValidateOrderOwnerAsync(1, string.Empty);

            // Assert
            Assert.False(result);
        }
    }
}
