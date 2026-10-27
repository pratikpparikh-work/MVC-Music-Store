using MvcMusicStore.Domain.Entities;

namespace MvcMusicStore.Application.DTOs
{
    /// <summary>
    /// DTO for album data transfer between layers.
    /// </summary>
    public class AlbumDto
    {
        public int AlbumId { get; set; }
        public int GenreId { get; set; }
        public int ArtistId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string AlbumArtUrl { get; set; } = string.Empty;
        public string? GenreName { get; set; }
        public string? ArtistName { get; set; }
    }

    /// <summary>
    /// DTO for genre data transfer.
    /// </summary>
    public class GenreDto
    {
        public int GenreId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<AlbumDto> Albums { get; set; } = new();
    }

    /// <summary>
    /// DTO for artist data transfer.
    /// </summary>
    public class ArtistDto
    {
        public int ArtistId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for shopping cart view model.
    /// Replaces legacy ShoppingCartViewModel.
    /// </summary>
    public class ShoppingCartDto
    {
        public List<CartItemDto> CartItems { get; set; } = new();
        public decimal CartTotal { get; set; }
    }

    /// <summary>
    /// DTO for individual cart item.
    /// </summary>
    public class CartItemDto
    {
        public int RecordId { get; set; }
        public string CartId { get; set; } = string.Empty;
        public int AlbumId { get; set; }
        public int Count { get; set; }
        public DateTime DateCreated { get; set; }
        public string AlbumTitle { get; set; } = string.Empty;
        public decimal AlbumPrice { get; set; }
        public string AlbumArtUrl { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for cart remove operation response (AJAX).
    /// Replaces legacy ShoppingCartRemoveViewModel.
    /// </summary>
    public class CartRemoveDto
    {
        public string Message { get; set; } = string.Empty;
        public decimal CartTotal { get; set; }
        public int CartCount { get; set; }
        public int ItemCount { get; set; }
        public int DeleteId { get; set; }
    }

    /// <summary>
    /// DTO for order creation.
    /// </summary>
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
