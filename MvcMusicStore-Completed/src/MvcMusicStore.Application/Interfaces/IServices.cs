using MvcMusicStore.Application.DTOs;
using MvcMusicStore.Domain.Entities;

namespace MvcMusicStore.Application.Interfaces
{
    /// <summary>
    /// Service interface for album operations.
    /// </summary>
    public interface IAlbumService
    {
        Task<IEnumerable<AlbumDto>> GetTopSellingAlbumsAsync(int count);
        Task<IEnumerable<AlbumDto>> GetAllAlbumsAsync();
        Task<AlbumDto?> GetAlbumByIdAsync(int id);
        Task<AlbumDto> CreateAlbumAsync(AlbumDto albumDto);
        Task UpdateAlbumAsync(AlbumDto albumDto);
        Task DeleteAlbumAsync(int id);
    }

    /// <summary>
    /// Service interface for genre operations.
    /// </summary>
    public interface IGenreService
    {
        Task<IEnumerable<GenreDto>> GetAllGenresAsync();
        Task<GenreDto?> GetGenreByNameAsync(string name);
        Task<GenreDto?> GetGenreWithAlbumsAsync(string name);
    }

    /// <summary>
    /// Service interface for artist operations.
    /// </summary>
    public interface IArtistService
    {
        Task<IEnumerable<ArtistDto>> GetAllArtistsAsync();
    }

    /// <summary>
    /// Service interface for shopping cart operations.
    /// Replaces legacy ShoppingCart model class.
    /// </summary>
    public interface IShoppingCartService
    {
        Task<ShoppingCartDto> GetCartAsync(string cartId);
        Task AddToCartAsync(string cartId, int albumId);
        Task<int> RemoveFromCartAsync(string cartId, int recordId);
        Task<int> GetCartCountAsync(string cartId);
        Task<decimal> GetCartTotalAsync(string cartId);
        Task MigrateCartAsync(string oldCartId, string newCartId);
        Task<int> CreateOrderAsync(string cartId, Order order);
        Task<string?> GetAlbumTitleByCartRecordIdAsync(string cartId, int recordId);
    }

    /// <summary>
    /// Service interface for order operations.
    /// </summary>
    public interface IOrderService
    {
        Task<bool> ValidateOrderOwnerAsync(int orderId, string username);
    }
}
