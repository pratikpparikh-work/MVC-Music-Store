using System.Threading.Tasks;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Domain.Interfaces;

namespace MvcMusicStore.Application.Services
{
    /// <summary>
    /// Application service for order operations.
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetByIdAsync(orderId);
        }

        public async Task<bool> IsOrderOwnedByUserAsync(int orderId, string username)
        {
            return await _orderRepository.IsOrderOwnedByUserAsync(orderId, username);
        }
    }
}
