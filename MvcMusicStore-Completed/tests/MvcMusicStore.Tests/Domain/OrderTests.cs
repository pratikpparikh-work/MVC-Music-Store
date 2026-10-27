using MvcMusicStore.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace MvcMusicStore.Tests.Domain
{
    public class OrderTests
    {
        [Fact]
        public void Order_DefaultConstructor_InitializesProperties()
        {
            // Arrange & Act
            var order = new Order();

            // Assert
            Assert.Equal(0, order.OrderId);
            Assert.Equal(default(DateTime), order.OrderDate);
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
        public void Order_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var orderDate = DateTime.UtcNow;

            // Act
            var order = new Order
            {
                OrderId = 1,
                OrderDate = orderDate,
                Username = "testuser",
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Main St",
                City = "Anytown",
                State = "CA",
                PostalCode = "12345",
                Country = "USA",
                Phone = "555-1234",
                Email = "john@example.com",
                Total = 29.99m
            };

            // Assert
            Assert.Equal(1, order.OrderId);
            Assert.Equal(orderDate, order.OrderDate);
            Assert.Equal("testuser", order.Username);
            Assert.Equal("John", order.FirstName);
            Assert.Equal("Doe", order.LastName);
            Assert.Equal("123 Main St", order.Address);
            Assert.Equal("Anytown", order.City);
            Assert.Equal("CA", order.State);
            Assert.Equal("12345", order.PostalCode);
            Assert.Equal("USA", order.Country);
            Assert.Equal("555-1234", order.Phone);
            Assert.Equal("john@example.com", order.Email);
            Assert.Equal(29.99m, order.Total);
        }

        [Fact]
        public void Order_FirstNameValidation_Required()
        {
            // Arrange
            var order = new Order { FirstName = string.Empty };
            var context = new ValidationContext(order) { MemberName = "FirstName" };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(order.FirstName, context, results);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage == "First Name is required");
        }

        [Fact]
        public void Order_EmailValidation_InvalidFormat()
        {
            // Arrange
            var order = new Order { Email = "invalid-email" };
            var context = new ValidationContext(order) { MemberName = "Email" };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(order.Email, context, results);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Order_EmailValidation_ValidFormat()
        {
            // Arrange
            var order = new Order { Email = "test@example.com" };
            var context = new ValidationContext(order) { MemberName = "Email" };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(order.Email, context, results);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Order_OrderDetails_CanAddItems()
        {
            // Arrange
            var order = new Order { OrderId = 1 };
            var detail = new OrderDetail { OrderDetailId = 1, OrderId = 1, AlbumId = 1, Quantity = 2, UnitPrice = 9.99m };

            // Act
            order.OrderDetails.Add(detail);

            // Assert
            Assert.Single(order.OrderDetails);
            Assert.Equal(detail, order.OrderDetails[0]);
        }

        [Fact]
        public void Order_Total_CanBeCalculated()
        {
            // Arrange
            var order = new Order { OrderId = 1 };
            order.OrderDetails.Add(new OrderDetail { Quantity = 2, UnitPrice = 9.99m });
            order.OrderDetails.Add(new OrderDetail { Quantity = 1, UnitPrice = 14.99m });

            // Act
            order.Total = order.OrderDetails.Sum(od => od.Quantity * od.UnitPrice);

            // Assert
            Assert.Equal(34.97m, order.Total);
        }
    }
}
