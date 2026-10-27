using MvcMusicStore.Domain.Entities;
using Xunit;

namespace MvcMusicStore.Tests.Domain
{
    public class OrderDetailTests
    {
        [Fact]
        public void OrderDetail_DefaultConstructor_InitializesProperties()
        {
            // Arrange & Act
            var detail = new OrderDetail();

            // Assert
            Assert.Equal(0, detail.OrderDetailId);
            Assert.Equal(0, detail.OrderId);
            Assert.Equal(0, detail.AlbumId);
            Assert.Equal(0, detail.Quantity);
            Assert.Equal(0m, detail.UnitPrice);
            Assert.Null(detail.Order);
            Assert.Null(detail.Album);
        }

        [Fact]
        public void OrderDetail_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var order = new Order { OrderId = 1 };
            var album = new Album { AlbumId = 1, Title = "Test Album" };

            // Act
            var detail = new OrderDetail
            {
                OrderDetailId = 1,
                OrderId = 1,
                AlbumId = 1,
                Quantity = 3,
                UnitPrice = 9.99m,
                Order = order,
                Album = album
            };

            // Assert
            Assert.Equal(1, detail.OrderDetailId);
            Assert.Equal(1, detail.OrderId);
            Assert.Equal(1, detail.AlbumId);
            Assert.Equal(3, detail.Quantity);
            Assert.Equal(9.99m, detail.UnitPrice);
            Assert.Equal(order, detail.Order);
            Assert.Equal(album, detail.Album);
        }

        [Fact]
        public void OrderDetail_LineTotal_CalculatedCorrectly()
        {
            // Arrange
            var detail = new OrderDetail
            {
                Quantity = 3,
                UnitPrice = 9.99m
            };

            // Act
            var lineTotal = detail.Quantity * detail.UnitPrice;

            // Assert
            Assert.Equal(29.97m, lineTotal);
        }

        [Fact]
        public void OrderDetail_Quantity_CanBeZero()
        {
            // Arrange & Act
            var detail = new OrderDetail { Quantity = 0, UnitPrice = 9.99m };

            // Assert
            Assert.Equal(0, detail.Quantity);
            Assert.Equal(0m, detail.Quantity * detail.UnitPrice);
        }

        [Fact]
        public void OrderDetail_UnitPrice_CanBeDecimal()
        {
            // Arrange & Act
            var detail = new OrderDetail { Quantity = 1, UnitPrice = 14.99m };

            // Assert
            Assert.Equal(14.99m, detail.UnitPrice);
        }
    }
}
