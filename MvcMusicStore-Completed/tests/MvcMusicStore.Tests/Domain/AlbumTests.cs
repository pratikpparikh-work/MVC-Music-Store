using MvcMusicStore.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace MvcMusicStore.Tests.Domain
{
    public class AlbumTests
    {
        [Fact]
        public void Album_DefaultConstructor_InitializesProperties()
        {
            // Arrange & Act
            var album = new Album();

            // Assert
            Assert.Equal(0, album.AlbumId);
            Assert.Equal(0, album.GenreId);
            Assert.Equal(0, album.ArtistId);
            Assert.Equal(string.Empty, album.Title);
            Assert.Equal(0m, album.Price);
            Assert.Equal(string.Empty, album.AlbumArtUrl);
            Assert.Null(album.Genre);
            Assert.Null(album.Artist);
            Assert.NotNull(album.OrderDetails);
            Assert.Empty(album.OrderDetails);
        }

        [Fact]
        public void Album_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var genre = new Genre { GenreId = 1, Name = "Rock" };
            var artist = new Artist { ArtistId = 1, Name = "Test Artist" };

            // Act
            var album = new Album
            {
                AlbumId = 1,
                GenreId = 1,
                ArtistId = 1,
                Title = "Test Album",
                Price = 9.99m,
                AlbumArtUrl = "/Content/Images/test.jpg",
                Genre = genre,
                Artist = artist
            };

            // Assert
            Assert.Equal(1, album.AlbumId);
            Assert.Equal(1, album.GenreId);
            Assert.Equal(1, album.ArtistId);
            Assert.Equal("Test Album", album.Title);
            Assert.Equal(9.99m, album.Price);
            Assert.Equal("/Content/Images/test.jpg", album.AlbumArtUrl);
            Assert.Equal(genre, album.Genre);
            Assert.Equal(artist, album.Artist);
        }

        [Fact]
        public void Album_TitleValidation_RequiredAttribute()
        {
            // Arrange
            var album = new Album { Title = string.Empty, Price = 9.99m };
            var context = new ValidationContext(album) { MemberName = "Title" };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(album.Title, context, results);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage == "An Album Title is required");
        }

        [Fact]
        public void Album_PriceValidation_RangeAttribute()
        {
            // Arrange
            var album = new Album { Title = "Test", Price = 0m };
            var context = new ValidationContext(album) { MemberName = "Price" };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(album.Price, context, results);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Album_PriceValidation_ValidPrice()
        {
            // Arrange
            var album = new Album { Title = "Test", Price = 9.99m };
            var context = new ValidationContext(album) { MemberName = "Price" };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(album.Price, context, results);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Album_OrderDetails_CanAddItems()
        {
            // Arrange
            var album = new Album { AlbumId = 1, Title = "Test Album" };
            var orderDetail = new OrderDetail { OrderDetailId = 1, AlbumId = 1, Quantity = 2 };

            // Act
            album.OrderDetails.Add(orderDetail);

            // Assert
            Assert.Single(album.OrderDetails);
            Assert.Equal(orderDetail, album.OrderDetails[0]);
        }

        [Fact]
        public void Album_TitleMaxLength_160Characters()
        {
            // Arrange
            var longTitle = new string('A', 161);
            var album = new Album { Title = longTitle, Price = 9.99m };
            var context = new ValidationContext(album) { MemberName = "Title" };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(album.Title, context, results);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Album_PriceMaxValue_100()
        {
            // Arrange
            var album = new Album { Title = "Test", Price = 100.01m };
            var context = new ValidationContext(album) { MemberName = "Price" };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(album.Price, context, results);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Album_PriceMinValue_001()
        {
            // Arrange
            var album = new Album { Title = "Test", Price = 0.01m };
            var context = new ValidationContext(album) { MemberName = "Price" };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(album.Price, context, results);

            // Assert
            Assert.True(isValid);
        }
    }
}
