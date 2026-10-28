using System.Threading.Tasks;
using MvcMusicStore.Domain.Entities;

namespace MvcMusicStore.Application.Interfaces
{
    /// <summary>
    /// Service interface for order-related business operations.
    /// </summary>
    public interface IOrderService
    {
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<bool> IsOrderOwnedByUserAsync(int orderId, string username);
    }
}
