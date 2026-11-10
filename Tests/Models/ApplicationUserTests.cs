using Xunit;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Models
{
    public class ApplicationUserTests
    {
        [Fact]
        public void ApplicationUser_DefaultConstructor_CreatesInstance()
        {
            var user = new ApplicationUser();
            Assert.NotNull(user);
        }

        [Fact]
        public void ApplicationUser_SetUserName_ReturnsCorrectValue()
        {
            var user = new ApplicationUser();
            user.UserName = "testuser";
            Assert.Equal("testuser", user.UserName);
        }

        [Fact]
        public void ApplicationUser_SetEmail_ReturnsCorrectValue()
        {
            var user = new ApplicationUser();
            user.Email = "test@example.com";
            Assert.Equal("test@example.com", user.Email);
        }

        [Fact]
        public void ApplicationUser_SetId_ReturnsCorrectValue()
        {
            var user = new ApplicationUser();
            user.Id = "user-id-123";
            Assert.Equal("user-id-123", user.Id);
        }

        [Fact]
        public void ApplicationUser_InheritsFromIdentityUser_IsTrue()
        {
            var user = new ApplicationUser();
            Assert.IsAssignableFrom<Microsoft.AspNetCore.Identity.IdentityUser>(user);
        }

        [Fact]
        public void ApplicationUser_InitializeWithValues_PropertiesAreSet()
        {
            var user = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@example.com",
                Id = "admin-id"
            };
            Assert.Equal("admin", user.UserName);
            Assert.Equal("admin@example.com", user.Email);
            Assert.Equal("admin-id", user.Id);
        }

        [Fact]
        public void ApplicationUser_SetPhoneNumber_ReturnsCorrectValue()
        {
            var user = new ApplicationUser();
            user.PhoneNumber = "555-1234";
            Assert.Equal("555-1234", user.PhoneNumber);
        }

        [Fact]
        public void ApplicationUser_SetEmailConfirmed_ReturnsCorrectValue()
        {
            var user = new ApplicationUser();
            user.EmailConfirmed = true;
            Assert.True(user.EmailConfirmed);
        }
    }
}
