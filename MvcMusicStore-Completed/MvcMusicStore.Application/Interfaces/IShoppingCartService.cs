using System.Collections.Generic;
using System.Threading.Tasks;
using MvcMusicStore.Domain.Entities;

namespace MvcMusicStore.Application.Interfaces
{
    /// <summary>
    /// Service interface for shopping cart business operations.
    /// Replaces the legacy ShoppingCart model that coupled directly to HttpContext.
    /// </summary>
    public interface IShoppingCartService
    {
        Task<List<Cart>> GetCartItemsAsync(string cartId);
        Task AddToCartAsync(string cartId, int albumId);
        Task<int> RemoveFromCartAsync(string cartId, int recordId);
        Task<int> GetCartCountAsync(string cartId);
        Task<decimal> GetCartTotalAsync(string cartId);
        Task MigrateCartAsync(string oldCartId, string newCartId);
        Task EmptyCartAsync(string cartId);
        Task<int> CreateOrderAsync(string cartId, Order order);
    }
}
