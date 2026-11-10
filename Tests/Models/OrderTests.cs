using System;
using System.Collections.Generic;
using Xunit;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Models
{
    public class OrderTests
    {
        [Fact]
        public void Order_DefaultConstructor_SetsDefaultValues()
        {
            var order = new Order();
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
        public void Order_SetOrderId_ReturnsCorrectValue()
        {
            var order = new Order();
            order.OrderId = 100;
            Assert.Equal(100, order.OrderId);
        }

        [Fact]
        public void Order_SetUsername_ReturnsCorrectValue()
        {
            var order = new Order();
            order.Username = "testuser";
            Assert.Equal("testuser", order.Username);
        }

        [Fact]
        public void Order_SetOrderDate_ReturnsCorrectValue()
        {
            var order = new Order();
            var date = new DateTime(2024, 1, 15);
            order.OrderDate = date;
            Assert.Equal(date, order.OrderDate);
        }

        [Fact]
        public void Order_SetFirstName_ReturnsCorrectValue()
        {
            var order = new Order();
            order.FirstName = "John";
            Assert.Equal("John", order.FirstName);
        }

        [Fact]
        public void Order_SetLastName_ReturnsCorrectValue()
        {
            var order = new Order();
            order.LastName = "Doe";
            Assert.Equal("Doe", order.LastName);
        }

        [Fact]
        public void Order_SetAddress_ReturnsCorrectValue()
        {
            var order = new Order();
            order.Address = "123 Main St";
            Assert.Equal("123 Main St", order.Address);
        }

        [Fact]
        public void Order_SetCity_ReturnsCorrectValue()
        {
            var order = new Order();
            order.City = "Seattle";
            Assert.Equal("Seattle", order.City);
        }

        [Fact]
        public void Order_SetState_ReturnsCorrectValue()
        {
            var order = new Order();
            order.State = "WA";
            Assert.Equal("WA", order.State);
        }

        [Fact]
        public void Order_SetPostalCode_ReturnsCorrectValue()
        {
            var order = new Order();
            order.PostalCode = "98101";
            Assert.Equal("98101", order.PostalCode);
        }

        [Fact]
        public void Order_SetCountry_ReturnsCorrectValue()
        {
            var order = new Order();
            order.Country = "USA";
            Assert.Equal("USA", order.Country);
        }

        [Fact]
        public void Order_SetPhone_ReturnsCorrectValue()
        {
            var order = new Order();
            order.Phone = "555-1234";
            Assert.Equal("555-1234", order.Phone);
        }

        [Fact]
        public void Order_SetEmail_ReturnsCorrectValue()
        {
            var order = new Order();
            order.Email = "test@example.com";
            Assert.Equal("test@example.com", order.Email);
        }

        [Fact]
        public void Order_SetTotal_ReturnsCorrectValue()
        {
            var order = new Order();
            order.Total = 49.99m;
            Assert.Equal(49.99m, order.Total);
        }

        [Fact]
        public void Order_AddOrderDetail_OrderDetailIsInList()
        {
            var order = new Order { OrderId = 1 };
            var detail = new OrderDetail { OrderDetailId = 1, OrderId = 1 };
            order.OrderDetails.Add(detail);
            Assert.Single(order.OrderDetails);
        }

        [Fact]
        public void Order_InitializeWithAllValues_PropertiesAreSet()
        {
            var order = new Order
            {
                OrderId = 1,
                Username = "john.doe",
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Main St",
                City = "Seattle",
                State = "WA",
                PostalCode = "98101",
                Country = "USA",
                Phone = "555-1234",
                Email = "john@example.com",
                Total = 99.99m
            };

            Assert.Equal(1, order.OrderId);
            Assert.Equal("john.doe", order.Username);
            Assert.Equal("John", order.FirstName);
            Assert.Equal("Doe", order.LastName);
            Assert.Equal("123 Main St", order.Address);
            Assert.Equal("Seattle", order.City);
            Assert.Equal("WA", order.State);
            Assert.Equal("98101", order.PostalCode);
            Assert.Equal("USA", order.Country);
            Assert.Equal("555-1234", order.Phone);
            Assert.Equal("john@example.com", order.Email);
            Assert.Equal(99.99m, order.Total);
        }

        [Fact]
        public void Order_OrderDetails_CanHoldMultipleItems()
        {
            var order = new Order { OrderId = 1 };
            order.OrderDetails.Add(new OrderDetail { OrderDetailId = 1 });
            order.OrderDetails.Add(new OrderDetail { OrderDetailId = 2 });
            Assert.Equal(2, order.OrderDetails.Count);
        }

        [Fact]
        public void Order_SetTotalToZero_ReturnsZero()
        {
            var order = new Order { Total = 50m };
            order.Total = 0m;
            Assert.Equal(0m, order.Total);
        }
    }
}
