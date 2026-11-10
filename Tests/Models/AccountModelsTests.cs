using System;
using Xunit;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Models
{
    public class AccountModelsTests
    {
        // ChangePasswordModel Tests
        [Fact]
        public void ChangePasswordModel_DefaultConstructor_SetsDefaultValues()
        {
            var model = new ChangePasswordModel();
            Assert.Equal(string.Empty, model.OldPassword);
            Assert.Equal(string.Empty, model.NewPassword);
            Assert.Equal(string.Empty, model.ConfirmPassword);
        }

        [Fact]
        public void ChangePasswordModel_SetOldPassword_ReturnsCorrectValue()
        {
            var model = new ChangePasswordModel();
            model.OldPassword = "OldPass123";
            Assert.Equal("OldPass123", model.OldPassword);
        }

        [Fact]
        public void ChangePasswordModel_SetNewPassword_ReturnsCorrectValue()
        {
            var model = new ChangePasswordModel();
            model.NewPassword = "NewPass456";
            Assert.Equal("NewPass456", model.NewPassword);
        }

        [Fact]
        public void ChangePasswordModel_SetConfirmPassword_ReturnsCorrectValue()
        {
            var model = new ChangePasswordModel();
            model.ConfirmPassword = "NewPass456";
            Assert.Equal("NewPass456", model.ConfirmPassword);
        }

        [Fact]
        public void ChangePasswordModel_InitializeWithValues_PropertiesAreSet()
        {
            var model = new ChangePasswordModel
            {
                OldPassword = "old",
                NewPassword = "newpass",
                ConfirmPassword = "newpass"
            };
            Assert.Equal("old", model.OldPassword);
            Assert.Equal("newpass", model.NewPassword);
            Assert.Equal("newpass", model.ConfirmPassword);
        }

        // LogOnModel Tests
        [Fact]
        public void LogOnModel_DefaultConstructor_SetsDefaultValues()
        {
            var model = new LogOnModel();
            Assert.Equal(string.Empty, model.UserName);
            Assert.Equal(string.Empty, model.Password);
            Assert.False(model.RememberMe);
        }

        [Fact]
        public void LogOnModel_SetUserName_ReturnsCorrectValue()
        {
            var model = new LogOnModel();
            model.UserName = "testuser";
            Assert.Equal("testuser", model.UserName);
        }

        [Fact]
        public void LogOnModel_SetPassword_ReturnsCorrectValue()
        {
            var model = new LogOnModel();
            model.Password = "password123";
            Assert.Equal("password123", model.Password);
        }

        [Fact]
        public void LogOnModel_SetRememberMeToTrue_ReturnsTrue()
        {
            var model = new LogOnModel();
            model.RememberMe = true;
            Assert.True(model.RememberMe);
        }

        [Fact]
        public void LogOnModel_SetRememberMeToFalse_ReturnsFalse()
        {
            var model = new LogOnModel { RememberMe = true };
            model.RememberMe = false;
            Assert.False(model.RememberMe);
        }

        [Fact]
        public void LogOnModel_InitializeWithValues_PropertiesAreSet()
        {
            var model = new LogOnModel
            {
                UserName = "admin",
                Password = "admin123",
                RememberMe = true
            };
            Assert.Equal("admin", model.UserName);
            Assert.Equal("admin123", model.Password);
            Assert.True(model.RememberMe);
        }

        // RegisterModel Tests
        [Fact]
        public void RegisterModel_DefaultConstructor_SetsDefaultValues()
        {
            var model = new RegisterModel();
            Assert.Equal(string.Empty, model.UserName);
            Assert.Equal(string.Empty, model.Email);
            Assert.Equal(string.Empty, model.Password);
            Assert.Equal(string.Empty, model.ConfirmPassword);
        }

        [Fact]
        public void RegisterModel_SetUserName_ReturnsCorrectValue()
        {
            var model = new RegisterModel();
            model.UserName = "newuser";
            Assert.Equal("newuser", model.UserName);
        }

        [Fact]
        public void RegisterModel_SetEmail_ReturnsCorrectValue()
        {
            var model = new RegisterModel();
            model.Email = "user@example.com";
            Assert.Equal("user@example.com", model.Email);
        }

        [Fact]
        public void RegisterModel_SetPassword_ReturnsCorrectValue()
        {
            var model = new RegisterModel();
            model.Password = "securepass";
            Assert.Equal("securepass", model.Password);
        }

        [Fact]
        public void RegisterModel_SetConfirmPassword_ReturnsCorrectValue()
        {
            var model = new RegisterModel();
            model.ConfirmPassword = "securepass";
            Assert.Equal("securepass", model.ConfirmPassword);
        }

        [Fact]
        public void RegisterModel_InitializeWithValues_PropertiesAreSet()
        {
            var model = new RegisterModel
            {
                UserName = "newuser",
                Email = "new@example.com",
                Password = "pass123",
                ConfirmPassword = "pass123"
            };
            Assert.Equal("newuser", model.UserName);
            Assert.Equal("new@example.com", model.Email);
            Assert.Equal("pass123", model.Password);
            Assert.Equal("pass123", model.ConfirmPassword);
        }
    }
}
