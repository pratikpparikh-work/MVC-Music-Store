using MvcMusicStore.Domain.Entities;

namespace MvcMusicStore.Domain.Interfaces
{
    /// <summary>
    /// Generic repository interface for data access operations.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<int> SaveChangesAsync();
    }

    /// <summary>
    /// Album-specific repository interface.
    /// </summary>
    public interface IAlbumRepository : IRepository<Album>
    {
        Task<IEnumerable<Album>> GetTopSellingAlbumsAsync(int count);
        Task<IEnumerable<Album>> GetAlbumsWithDetailsAsync();
        Task<Album?> GetAlbumWithDetailsAsync(int id);
    }

    /// <summary>
    /// Genre-specific repository interface.
    /// </summary>
    public interface IGenreRepository : IRepository<Genre>
    {
        Task<Genre?> GetGenreByNameAsync(string name);
        Task<Genre?> GetGenreWithAlbumsAsync(string name);
    }

    /// <summary>
    /// Artist-specific repository interface.
    /// </summary>
    public interface IArtistRepository : IRepository<Artist>
    {
    }

    /// <summary>
    /// Cart-specific repository interface.
    /// </summary>
    public interface ICartRepository : IRepository<Cart>
    {
        Task<List<Cart>> GetCartItemsAsync(string cartId);
        Task<Cart?> GetCartItemAsync(string cartId, int albumId);
        Task<Cart?> GetCartItemByRecordIdAsync(string cartId, int recordId);
        Task<int> GetCartCountAsync(string cartId);
        Task<decimal> GetCartTotalAsync(string cartId);
        Task MigrateCartAsync(string oldCartId, string newCartId);
        Task EmptyCartAsync(string cartId);
    }

    /// <summary>
    /// Order-specific repository interface.
    /// </summary>
    public interface IOrderRepository : IRepository<Order>
    {
        Task<bool> ValidateOrderOwnerAsync(int orderId, string username);
    }

    /// <summary>
    /// OrderDetail-specific repository interface.
    /// </summary>
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
    }
}
