using MvcMusicStore.Application.DTOs;
using Xunit;

namespace MvcMusicStore.Tests.Application
{
    public class DtoTests
    {
        // ─── AlbumDto ───────────────────────────────────────────────────────────────

        [Fact]
        public void AlbumDto_DefaultConstructor_InitializesProperties()
        {
            var dto = new AlbumDto();

            Assert.Equal(0, dto.AlbumId);
            Assert.Equal(0, dto.GenreId);
            Assert.Equal(0, dto.ArtistId);
            Assert.Equal(string.Empty, dto.Title);
            Assert.Equal(0m, dto.Price);
            Assert.Equal(string.Empty, dto.AlbumArtUrl);
            Assert.Null(dto.GenreName);
            Assert.Null(dto.ArtistName);
        }

        [Fact]
        public void AlbumDto_SetProperties_ReturnsCorrectValues()
        {
            var dto = new AlbumDto
            {
                AlbumId = 1,
                GenreId = 2,
                ArtistId = 3,
                Title = "Test Album",
                Price = 9.99m,
                AlbumArtUrl = "/images/test.jpg",
                GenreName = "Rock",
                ArtistName = "Test Artist"
            };

            Assert.Equal(1, dto.AlbumId);
            Assert.Equal(2, dto.GenreId);
            Assert.Equal(3, dto.ArtistId);
            Assert.Equal("Test Album", dto.Title);
            Assert.Equal(9.99m, dto.Price);
            Assert.Equal("/images/test.jpg", dto.AlbumArtUrl);
            Assert.Equal("Rock", dto.GenreName);
            Assert.Equal("Test Artist", dto.ArtistName);
        }

        // ─── GenreDto ───────────────────────────────────────────────────────────────

        [Fact]
        public void GenreDto_DefaultConstructor_InitializesProperties()
        {
            var dto = new GenreDto();

            Assert.Equal(0, dto.GenreId);
            Assert.Equal(string.Empty, dto.Name);
            Assert.Equal(string.Empty, dto.Description);
            Assert.NotNull(dto.Albums);
            Assert.Empty(dto.Albums);
        }

        [Fact]
        public void GenreDto_SetProperties_ReturnsCorrectValues()
        {
            var dto = new GenreDto
            {
                GenreId = 1,
                Name = "Rock",
                Description = "Rock music"
            };

            Assert.Equal(1, dto.GenreId);
            Assert.Equal("Rock", dto.Name);
            Assert.Equal("Rock music", dto.Description);
        }

        [Fact]
        public void GenreDto_Albums_CanAddItems()
        {
            var dto = new GenreDto();
            dto.Albums.Add(new AlbumDto { AlbumId = 1, Title = "Test" });

            Assert.Single(dto.Albums);
        }

        // ─── ArtistDto ──────────────────────────────────────────────────────────────

        [Fact]
        public void ArtistDto_DefaultConstructor_InitializesProperties()
        {
            var dto = new ArtistDto();

            Assert.Equal(0, dto.ArtistId);
            Assert.Equal(string.Empty, dto.Name);
        }

        [Fact]
        public void ArtistDto_SetProperties_ReturnsCorrectValues()
        {
            var dto = new ArtistDto { ArtistId = 5, Name = "My Artist" };

            Assert.Equal(5, dto.ArtistId);
            Assert.Equal("My Artist", dto.Name);
        }

        // ─── ShoppingCartDto ────────────────────────────────────────────────────────

        [Fact]
        public void ShoppingCartDto_DefaultConstructor_InitializesProperties()
        {
            var dto = new ShoppingCartDto();

            Assert.NotNull(dto.CartItems);
            Assert.Empty(dto.CartItems);
            Assert.Equal(0m, dto.CartTotal);
        }

        [Fact]
        public void ShoppingCartDto_SetProperties_ReturnsCorrectValues()
        {
            var dto = new ShoppingCartDto
            {
                CartTotal = 29.97m
            };
            dto.CartItems.Add(new CartItemDto { RecordId = 1, Count = 3, AlbumPrice = 9.99m });

            Assert.Equal(29.97m, dto.CartTotal);
            Assert.Single(dto.CartItems);
        }

        // ─── CartItemDto ────────────────────────────────────────────────────────────

        [Fact]
        public void CartItemDto_DefaultConstructor_InitializesProperties()
        {
            var dto = new CartItemDto();

            Assert.Equal(0, dto.RecordId);
            Assert.Equal(string.Empty, dto.CartId);
            Assert.Equal(0, dto.AlbumId);
            Assert.Equal(0, dto.Count);
            Assert.Equal(default(DateTime), dto.DateCreated);
            Assert.Equal(string.Empty, dto.AlbumTitle);
            Assert.Equal(0m, dto.AlbumPrice);
            Assert.Equal(string.Empty, dto.AlbumArtUrl);
        }

        [Fact]
        public void CartItemDto_SetProperties_ReturnsCorrectValues()
        {
            var date = DateTime.UtcNow;
            var dto = new CartItemDto
            {
                RecordId = 1,
                CartId = "cart-123",
                AlbumId = 5,
                Count = 2,
                DateCreated = date,
                AlbumTitle = "Test Album",
                AlbumPrice = 9.99m,
                AlbumArtUrl = "/images/test.jpg"
            };

            Assert.Equal(1, dto.RecordId);
            Assert.Equal("cart-123", dto.CartId);
            Assert.Equal(5, dto.AlbumId);
            Assert.Equal(2, dto.Count);
            Assert.Equal(date, dto.DateCreated);
            Assert.Equal("Test Album", dto.AlbumTitle);
            Assert.Equal(9.99m, dto.AlbumPrice);
            Assert.Equal("/images/test.jpg", dto.AlbumArtUrl);
        }

        // ─── CartRemoveDto ──────────────────────────────────────────────────────────

        [Fact]
        public void CartRemoveDto_DefaultConstructor_InitializesProperties()
        {
            var dto = new CartRemoveDto();

            Assert.Equal(string.Empty, dto.Message);
            Assert.Equal(0m, dto.CartTotal);
            Assert.Equal(0, dto.CartCount);
            Assert.Equal(0, dto.ItemCount);
            Assert.Equal(0, dto.DeleteId);
        }

        [Fact]
        public void CartRemoveDto_SetProperties_ReturnsCorrectValues()
        {
            var dto = new CartRemoveDto
            {
                Message = "Item removed",
                CartTotal = 19.98m,
                CartCount = 2,
                ItemCount = 1,
                DeleteId = 42
            };

            Assert.Equal("Item removed", dto.Message);
            Assert.Equal(19.98m, dto.CartTotal);
            Assert.Equal(2, dto.CartCount);
            Assert.Equal(1, dto.ItemCount);
            Assert.Equal(42, dto.DeleteId);
        }

        // ─── OrderDto ───────────────────────────────────────────────────────────────

        [Fact]
        public void OrderDto_DefaultConstructor_InitializesProperties()
        {
            var dto = new OrderDto();

            Assert.Equal(0, dto.OrderId);
            Assert.Equal(string.Empty, dto.FirstName);
            Assert.Equal(string.Empty, dto.LastName);
            Assert.Equal(string.Empty, dto.Address);
            Assert.Equal(string.Empty, dto.City);
            Assert.Equal(string.Empty, dto.State);
            Assert.Equal(string.Empty, dto.PostalCode);
            Assert.Equal(string.Empty, dto.Country);
            Assert.Equal(string.Empty, dto.Phone);
            Assert.Equal(string.Empty, dto.Email);
            Assert.Equal(0m, dto.Total);
            Assert.Equal(default(DateTime), dto.OrderDate);
        }

        [Fact]
        public void OrderDto_SetProperties_ReturnsCorrectValues()
        {
            var date = DateTime.UtcNow;
            var dto = new OrderDto
            {
                OrderId = 1,
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Main St",
                City = "Anytown",
                State = "CA",
                PostalCode = "12345",
                Country = "USA",
                Phone = "555-1234",
                Email = "john@example.com",
                Total = 29.99m,
                OrderDate = date
            };

            Assert.Equal(1, dto.OrderId);
            Assert.Equal("John", dto.FirstName);
            Assert.Equal("Doe", dto.LastName);
            Assert.Equal("123 Main St", dto.Address);
            Assert.Equal("Anytown", dto.City);
            Assert.Equal("CA", dto.State);
            Assert.Equal("12345", dto.PostalCode);
            Assert.Equal("USA", dto.Country);
            Assert.Equal("555-1234", dto.Phone);
            Assert.Equal("john@example.com", dto.Email);
            Assert.Equal(29.99m, dto.Total);
            Assert.Equal(date, dto.OrderDate);
        }
    }
}
