using System.Threading.Tasks;
using MvcMusicStore.Domain.Entities;

namespace MvcMusicStore.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Order data access.
    /// </summary>
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int orderId);
        Task<bool> IsOrderOwnedByUserAsync(int orderId, string username);
        Task<Order> AddAsync(Order order);
        Task SaveChangesAsync();
    }
}
