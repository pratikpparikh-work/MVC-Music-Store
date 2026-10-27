using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Domain.Interfaces;
using MvcMusicStore.Infrastructure.Data;

namespace MvcMusicStore.Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository implementation using EF Core.
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly MusicStoreDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(MusicStoreDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Remove(T entity) => _dbSet.Remove(entity);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Album repository implementation.
    /// </summary>
    public class AlbumRepository : Repository<Album>, IAlbumRepository
    {
        public AlbumRepository(MusicStoreDbContext context) : base(context) { }

        public async Task<IEnumerable<Album>> GetTopSellingAlbumsAsync(int count)
        {
            return await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .OrderByDescending(a => a.OrderDetails.Count)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Album>> GetAlbumsWithDetailsAsync()
        {
            return await _context.Albums
                .Include(a => a.Genre)
                .Include(a => a.Artist)
                .ToListAsync();
        }

        public async Task<Album?> GetAlbumWithDetailsAsync(int id)
        {
            return await _context.Albums
                .Include(a => a.Genre)
                .Include(a => a.Artist)
                .FirstOrDefaultAsync(a => a.AlbumId == id);
        }
    }

    /// <summary>
    /// Genre repository implementation.
    /// </summary>
    public class GenreRepository : Repository<Genre>, IGenreRepository
    {
        public GenreRepository(MusicStoreDbContext context) : base(context) { }

        public async Task<Genre?> GetGenreByNameAsync(string name)
        {
            return await _context.Genres.FirstOrDefaultAsync(g => g.Name == name);
        }

        public async Task<Genre?> GetGenreWithAlbumsAsync(string name)
        {
            return await _context.Genres
                .Include(g => g.Albums)
                .ThenInclude(a => a.Artist)
                .FirstOrDefaultAsync(g => g.Name == name);
        }
    }

    /// <summary>
    /// Artist repository implementation.
    /// </summary>
    public class ArtistRepository : Repository<Artist>, IArtistRepository
    {
        public ArtistRepository(MusicStoreDbContext context) : base(context) { }
    }

    /// <summary>
    /// Cart repository implementation.
    /// Replaces legacy ShoppingCart model with session-based cart management.
    /// </summary>
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(MusicStoreDbContext context) : base(context) { }

        public async Task<List<Cart>> GetCartItemsAsync(string cartId)
        {
            return await _context.Carts
                .Include(c => c.Album)
                .Where(c => c.CartId == cartId)
                .ToListAsync();
        }

        public async Task<Cart?> GetCartItemAsync(string cartId, int albumId)
        {
            return await _context.Carts
                .SingleOrDefaultAsync(c => c.CartId == cartId && c.AlbumId == albumId);
        }

        public async Task<Cart?> GetCartItemByRecordIdAsync(string cartId, int recordId)
        {
            return await _context.Carts
                .Include(c => c.Album)
                .SingleOrDefaultAsync(c => c.CartId == cartId && c.RecordId == recordId);
        }

        public async Task<int> GetCartCountAsync(string cartId)
        {
            var count = await _context.Carts
                .Where(c => c.CartId == cartId)
                .SumAsync(c => (int?)c.Count);
            return count ?? 0;
        }

        public async Task<decimal> GetCartTotalAsync(string cartId)
        {
            var total = await _context.Carts
                .Include(c => c.Album)
                .Where(c => c.CartId == cartId)
                .SumAsync(c => (decimal?)(c.Count * c.Album!.Price));
            return total ?? decimal.Zero;
        }

        public async Task MigrateCartAsync(string oldCartId, string newCartId)
        {
            var cartItems = await _context.Carts
                .Where(c => c.CartId == oldCartId)
                .ToListAsync();

            foreach (var item in cartItems)
            {
                item.CartId = newCartId;
            }
            await _context.SaveChangesAsync();
        }

        public async Task EmptyCartAsync(string cartId)
        {
            var cartItems = await _context.Carts
                .Where(c => c.CartId == cartId)
                .ToListAsync();

            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Order repository implementation.
    /// </summary>
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(MusicStoreDbContext context) : base(context) { }

        public async Task<bool> ValidateOrderOwnerAsync(int orderId, string username)
        {
            return await _context.Orders
                .AnyAsync(o => o.OrderId == orderId && o.Username == username);
        }
    }

    /// <summary>
    /// OrderDetail repository implementation.
    /// </summary>
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(MusicStoreDbContext context) : base(context) { }
    }
}
