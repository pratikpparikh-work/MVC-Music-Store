using System.Collections.Generic;
using System.Threading.Tasks;
using MvcMusicStore.Domain.Entities;

namespace MvcMusicStore.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for shopping cart data access.
    /// </summary>
    public interface ICartRepository
    {
        Task<List<Cart>> GetCartItemsAsync(string cartId);
        Task<Cart?> GetCartItemAsync(string cartId, int albumId);
        Task<Cart?> GetCartItemByRecordIdAsync(int recordId);
        Task AddCartItemAsync(Cart cartItem);
        Task UpdateCartItemAsync(Cart cartItem);
        Task RemoveCartItemAsync(Cart cartItem);
        Task<int> GetCartCountAsync(string cartId);
        Task<decimal> GetCartTotalAsync(string cartId);
        Task MigrateCartAsync(string oldCartId, string newCartId);
        Task EmptyCartAsync(string cartId);
    }
}
