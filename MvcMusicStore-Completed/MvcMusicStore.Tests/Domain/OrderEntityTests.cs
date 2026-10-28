using System;
using System.Collections.Generic;
using MvcMusicStore.Domain.Entities;
using Xunit;

namespace MvcMusicStore.Tests.Domain
{
    /// <summary>
    /// Unit tests for the Order domain entity.
    /// </summary>
    public class OrderEntityTests
    {
        [Fact]
        public void Order_DefaultConstructor_InitializesProperties()
        {
            // Arrange & Act
            var order = new Order();

            // Assert
            Assert.Equal(0, order.OrderId);
            Assert.Equal(string.Empty, order.Username);
            Assert.Equal(string.Empty, order.FirstName);
            Assert.Equal(string.Empty, order.LastName);
            Assert.Equal(string.Empty, order.Address);
            Assert.Equal(string.Empty, order.City);
            Assert.Equal(string.Empty, order.State);
            Assert.Equal(string.Empty, order.PostalCode);
            Assert.Equal(string.Empty, order.Country);
            Assert.Equal(string.Empty, order.Phone);
            Assert.Equal(string.Empty, order.Email);
            Assert.Equal(0m, order.Total);
            Assert.NotNull(order.OrderDetails);
            Assert.Empty(order.OrderDetails);
        }

        [Fact]
        public void Order_SetOrderDate_ReturnsCorrectDate()
        {
            // Arrange
            var order = new Order();
            var expectedDate = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.Zero);

            // Act
            order.OrderDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, order.OrderDate);
        }

        [Fact]
        public void Order_SetUsername_ReturnsCorrectUsername()
        {
            // Arrange
            var order = new Order();

            // Act
            order.Username = "testuser";

            // Assert
            Assert.Equal("testuser", order.Username);
        }

        [Fact]
        public void Order_SetTotal_ReturnsCorrectTotal()
        {
            // Arrange
            var order = new Order();

            // Act
            order.Total = 99.99m;

            // Assert
            Assert.Equal(99.99m, order.Total);
        }

        [Fact]
        public void Order_AddOrderDetail_OrderDetailsContainsItem()
        {
            // Arrange
            var order = new Order();
            var detail = new OrderDetail { OrderDetailId = 1, Quantity = 2, UnitPrice = 9.99m };

            // Act
            order.OrderDetails.Add(detail);

            // Assert
            Assert.Single(order.OrderDetails);
            Assert.Equal(1, order.OrderDetails[0].OrderDetailId);
        }

        [Fact]
        public void Order_SetAllAddressFields_AllFieldsCorrect()
        {
            // Arrange & Act
            var order = new Order
            {
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Main St",
                City = "Springfield",
                State = "IL",
                PostalCode = "62701",
                Country = "USA",
                Phone = "555-1234",
                Email = "john.doe@example.com"
            };

            // Assert
            Assert.Equal("John", order.FirstName);
            Assert.Equal("Doe", order.LastName);
            Assert.Equal("123 Main St", order.Address);
            Assert.Equal("Springfield", order.City);
            Assert.Equal("IL", order.State);
            Assert.Equal("62701", order.PostalCode);
            Assert.Equal("USA", order.Country);
            Assert.Equal("555-1234", order.Phone);
            Assert.Equal("john.doe@example.com", order.Email);
        }

        [Fact]
        public void Order_AddMultipleOrderDetails_AllDetailsPresent()
        {
            // Arrange
            var order = new Order();

            // Act
            order.OrderDetails.Add(new OrderDetail { OrderDetailId = 1 });
            order.OrderDetails.Add(new OrderDetail { OrderDetailId = 2 });

            // Assert
            Assert.Equal(2, order.OrderDetails.Count);
        }
    }
}
