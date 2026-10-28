using System.Collections.Generic;
using MvcMusicStore.Web.Models;
using MvcMusicStore.Web.ViewModels;
using MvcMusicStore.Domain.Entities;
using Xunit;

namespace MvcMusicStore.Tests.ViewModels
{
    /// <summary>
    /// Unit tests for view model classes.
    /// </summary>
    public class ViewModelTests
    {
        // ---- ShoppingCartViewModel Tests ----

        [Fact]
        public void ShoppingCartViewModel_DefaultConstructor_InitializesProperties()
        {
            // Arrange & Act
            var vm = new ShoppingCartViewModel();

            // Assert
            Assert.NotNull(vm.CartItems);
            Assert.Empty(vm.CartItems);
            Assert.Equal(0m, vm.CartTotal);
        }

        [Fact]
        public void ShoppingCartViewModel_SetCartTotal_ReturnsCorrectTotal()
        {
            // Arrange
            var vm = new ShoppingCartViewModel();

            // Act
            vm.CartTotal = 49.99m;

            // Assert
            Assert.Equal(49.99m, vm.CartTotal);
        }

        [Fact]
        public void ShoppingCartViewModel_SetCartItems_ReturnsCorrectItems()
        {
            // Arrange
            var vm = new ShoppingCartViewModel();
            var items = new List<Cart>
            {
                new Cart { RecordId = 1, AlbumId = 1, Count = 2 },
                new Cart { RecordId = 2, AlbumId = 2, Count = 1 }
            };

            // Act
            vm.CartItems = items;

            // Assert
            Assert.Equal(2, vm.CartItems.Count);
        }

        // ---- ShoppingCartRemoveViewModel Tests ----

        [Fact]
        public void ShoppingCartRemoveViewModel_DefaultConstructor_InitializesProperties()
        {
            // Arrange & Act
            var vm = new ShoppingCartRemoveViewModel();

            // Assert
            Assert.Equal(string.Empty, vm.Message);
            Assert.Equal(0m, vm.CartTotal);
            Assert.Equal(0, vm.CartCount);
            Assert.Equal(0, vm.ItemCount);
            Assert.Equal(0, vm.DeleteId);
        }

        [Fact]
        public void ShoppingCartRemoveViewModel_SetMessage_ReturnsCorrectMessage()
        {
            // Arrange
            var vm = new ShoppingCartRemoveViewModel();

            // Act
            vm.Message = "Item removed successfully.";

            // Assert
            Assert.Equal("Item removed successfully.", vm.Message);
        }

        [Fact]
        public void ShoppingCartRemoveViewModel_SetAllProperties_AllPropertiesCorrect()
        {
            // Arrange & Act
            var vm = new ShoppingCartRemoveViewModel
            {
                Message = "Removed",
                CartTotal = 29.99m,
                CartCount = 3,
                ItemCount = 2,
                DeleteId = 5
            };

            // Assert
            Assert.Equal("Removed", vm.Message);
            Assert.Equal(29.99m, vm.CartTotal);
            Assert.Equal(3, vm.CartCount);
            Assert.Equal(2, vm.ItemCount);
            Assert.Equal(5, vm.DeleteId);
        }

        // ---- LoginViewModel Tests ----

        [Fact]
        public void LoginViewModel_DefaultConstructor_InitializesProperties()
        {
            // Arrange & Act
            var vm = new LoginViewModel();

            // Assert
            Assert.Equal(string.Empty, vm.UserName);
            Assert.Equal(string.Empty, vm.Password);
            Assert.False(vm.RememberMe);
        }

        [Fact]
        public void LoginViewModel_SetUserName_ReturnsCorrectUserName()
        {
            // Arrange
            var vm = new LoginViewModel();

            // Act
            vm.UserName = "testuser";

            // Assert
            Assert.Equal("testuser", vm.UserName);
        }

        [Fact]
        public void LoginViewModel_SetPassword_ReturnsCorrectPassword()
        {
            // Arrange
            var vm = new LoginViewModel();

            // Act
            vm.Password = "P@ssw0rd";

            // Assert
            Assert.Equal("P@ssw0rd", vm.Password);
        }

        [Fact]
        public void LoginViewModel_SetRememberMe_ReturnsCorrectValue()
        {
            // Arrange
            var vm = new LoginViewModel();

            // Act
            vm.RememberMe = true;

            // Assert
            Assert.True(vm.RememberMe);
        }

        // ---- RegisterViewModel Tests ----

        [Fact]
        public void RegisterViewModel_DefaultConstructor_InitializesProperties()
        {
            // Arrange & Act
            var vm = new RegisterViewModel();

            // Assert
            Assert.Equal(string.Empty, vm.UserName);
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.Password);
            Assert.Equal(string.Empty, vm.ConfirmPassword);
        }

        [Fact]
        public void RegisterViewModel_SetAllProperties_AllPropertiesCorrect()
        {
            // Arrange & Act
            var vm = new RegisterViewModel
            {
                UserName = "newuser",
                Email = "newuser@example.com",
                Password = "P@ssw0rd1",
                ConfirmPassword = "P@ssw0rd1"
            };

            // Assert
            Assert.Equal("newuser", vm.UserName);
            Assert.Equal("newuser@example.com", vm.Email);
            Assert.Equal("P@ssw0rd1", vm.Password);
            Assert.Equal("P@ssw0rd1", vm.ConfirmPassword);
        }

        // ---- ChangePasswordViewModel Tests ----

        [Fact]
        public void ChangePasswordViewModel_DefaultConstructor_InitializesProperties()
        {
            // Arrange & Act
            var vm = new ChangePasswordViewModel();

            // Assert
            Assert.Equal(string.Empty, vm.OldPassword);
            Assert.Equal(string.Empty, vm.NewPassword);
            Assert.Equal(string.Empty, vm.ConfirmPassword);
        }

        [Fact]
        public void ChangePasswordViewModel_SetAllProperties_AllPropertiesCorrect()
        {
            // Arrange & Act
            var vm = new ChangePasswordViewModel
            {
                OldPassword = "OldP@ss1",
                NewPassword = "NewP@ss1",
                ConfirmPassword = "NewP@ss1"
            };

            // Assert
            Assert.Equal("OldP@ss1", vm.OldPassword);
            Assert.Equal("NewP@ss1", vm.NewPassword);
            Assert.Equal("NewP@ss1", vm.ConfirmPassword);
        }
    }
}
