using System.ComponentModel.DataAnnotations;
using MvcMusicStore.Web.Models;
using Xunit;

namespace MvcMusicStore.Tests.Web.Models
{
    public class AccountViewModelsTests
    {
        // ─── LoginViewModel ──────────────────────────────────────────────────────────

        [Fact]
        public void LoginViewModel_DefaultConstructor_InitializesProperties()
        {
            var model = new LoginViewModel();

            Assert.Equal(string.Empty, model.UserName);
            Assert.Equal(string.Empty, model.Password);
            Assert.False(model.RememberMe);
        }

        [Fact]
        public void LoginViewModel_SetProperties_ReturnsCorrectValues()
        {
            var model = new LoginViewModel
            {
                UserName = "testuser",
                Password = "password123",
                RememberMe = true
            };

            Assert.Equal("testuser", model.UserName);
            Assert.Equal("password123", model.Password);
            Assert.True(model.RememberMe);
        }

        [Fact]
        public void LoginViewModel_UserName_Required()
        {
            var model = new LoginViewModel { UserName = string.Empty, Password = "pass" };
            var context = new ValidationContext(model) { MemberName = "UserName" };
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateProperty(model.UserName, context, results);

            Assert.False(isValid);
        }

        [Fact]
        public void LoginViewModel_Password_Required()
        {
            var model = new LoginViewModel { UserName = "user", Password = string.Empty };
            var context = new ValidationContext(model) { MemberName = "Password" };
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateProperty(model.Password, context, results);

            Assert.False(isValid);
        }

        [Fact]
        public void LoginViewModel_ValidModel_PassesValidation()
        {
            var model = new LoginViewModel { UserName = "testuser", Password = "password123" };
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isValid);
        }

        // ─── RegisterViewModel ───────────────────────────────────────────────────────

        [Fact]
        public void RegisterViewModel_DefaultConstructor_InitializesProperties()
        {
            var model = new RegisterViewModel();

            Assert.Equal(string.Empty, model.UserName);
            Assert.Equal(string.Empty, model.Email);
            Assert.Equal(string.Empty, model.Password);
            Assert.Equal(string.Empty, model.ConfirmPassword);
        }

        [Fact]
        public void RegisterViewModel_SetProperties_ReturnsCorrectValues()
        {
            var model = new RegisterViewModel
            {
                UserName = "newuser",
                Email = "newuser@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            Assert.Equal("newuser", model.UserName);
            Assert.Equal("newuser@example.com", model.Email);
            Assert.Equal("Password123!", model.Password);
            Assert.Equal("Password123!", model.ConfirmPassword);
        }

        [Fact]
        public void RegisterViewModel_PasswordTooShort_FailsValidation()
        {
            var model = new RegisterViewModel
            {
                UserName = "user",
                Email = "user@example.com",
                Password = "abc",
                ConfirmPassword = "abc"
            };
            var context = new ValidationContext(model) { MemberName = "Password" };
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateProperty(model.Password, context, results);

            Assert.False(isValid);
        }

        [Fact]
        public void RegisterViewModel_ValidModel_PassesValidation()
        {
            var model = new RegisterViewModel
            {
                UserName = "newuser",
                Email = "newuser@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isValid);
        }

        // ─── ChangePasswordViewModel ─────────────────────────────────────────────────

        [Fact]
        public void ChangePasswordViewModel_DefaultConstructor_InitializesProperties()
        {
            var model = new ChangePasswordViewModel();

            Assert.Equal(string.Empty, model.OldPassword);
            Assert.Equal(string.Empty, model.NewPassword);
            Assert.Equal(string.Empty, model.ConfirmPassword);
        }

        [Fact]
        public void ChangePasswordViewModel_SetProperties_ReturnsCorrectValues()
        {
            var model = new ChangePasswordViewModel
            {
                OldPassword = "OldPass123!",
                NewPassword = "NewPass123!",
                ConfirmPassword = "NewPass123!"
            };

            Assert.Equal("OldPass123!", model.OldPassword);
            Assert.Equal("NewPass123!", model.NewPassword);
            Assert.Equal("NewPass123!", model.ConfirmPassword);
        }

        [Fact]
        public void ChangePasswordViewModel_NewPasswordTooShort_FailsValidation()
        {
            var model = new ChangePasswordViewModel
            {
                OldPassword = "OldPass123!",
                NewPassword = "abc",
                ConfirmPassword = "abc"
            };
            var context = new ValidationContext(model) { MemberName = "NewPassword" };
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateProperty(model.NewPassword, context, results);

            Assert.False(isValid);
        }

        [Fact]
        public void ChangePasswordViewModel_ValidModel_PassesValidation()
        {
            var model = new ChangePasswordViewModel
            {
                OldPassword = "OldPass123!",
                NewPassword = "NewPass123!",
                ConfirmPassword = "NewPass123!"
            };
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isValid);
        }
    }
}
