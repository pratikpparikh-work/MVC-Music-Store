using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Domain.Interfaces;
using MvcMusicStore.Infrastructure.Data;

namespace MvcMusicStore.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of IOrderRepository.
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly MusicStoreDbContext _context;

        public OrderRepository(MusicStoreDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Album)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<bool> IsOrderOwnedByUserAsync(int orderId, string username)
        {
            return await _context.Orders
                .AnyAsync(o => o.OrderId == orderId && o.Username == username);
        }

        public async Task<Order> AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
