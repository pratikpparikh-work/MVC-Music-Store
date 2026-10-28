using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Domain.Interfaces;
using MvcMusicStore.Infrastructure.Data;

namespace MvcMusicStore.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of ICartRepository.
    /// Replaces the legacy ShoppingCart model that used HttpContextBase directly.
    /// </summary>
    public class CartRepository : ICartRepository
    {
        private readonly MusicStoreDbContext _context;

        public CartRepository(MusicStoreDbContext context)
        {
            _context = context;
        }

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

        public async Task<Cart?> GetCartItemByRecordIdAsync(int recordId)
        {
            return await _context.Carts
                .Include(c => c.Album)
                .SingleOrDefaultAsync(c => c.RecordId == recordId);
        }

        public async Task AddCartItemAsync(Cart cartItem)
        {
            _context.Carts.Add(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(Cart cartItem)
        {
            _context.Entry(cartItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCartItemAsync(Cart cartItem)
        {
            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCartCountAsync(string cartId)
        {
            int? count = await _context.Carts
                .Where(c => c.CartId == cartId)
                .SumAsync(c => (int?)c.Count);
            return count ?? 0;
        }

        public async Task<decimal> GetCartTotalAsync(string cartId)
        {
            decimal? total = await _context.Carts
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
}
