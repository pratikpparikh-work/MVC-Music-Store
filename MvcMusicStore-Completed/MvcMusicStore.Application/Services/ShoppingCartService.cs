using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Domain.Interfaces;

namespace MvcMusicStore.Application.Services
{
    /// <summary>
    /// Application service for shopping cart operations.
    /// Replaces the legacy ShoppingCart model that used HttpContextBase directly.
    /// Cart ID management is now handled by the Web layer (session/cookie).
    /// </summary>
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IAlbumRepository _albumRepository;
        private readonly IOrderRepository _orderRepository;

        public ShoppingCartService(
            ICartRepository cartRepository,
            IAlbumRepository albumRepository,
            IOrderRepository orderRepository)
        {
            _cartRepository = cartRepository;
            _albumRepository = albumRepository;
            _orderRepository = orderRepository;
        }

        public async Task<List<Cart>> GetCartItemsAsync(string cartId)
        {
            return await _cartRepository.GetCartItemsAsync(cartId);
        }

        public async Task AddToCartAsync(string cartId, int albumId)
        {
            var cartItem = await _cartRepository.GetCartItemAsync(cartId, albumId);

            if (cartItem == null)
            {
                cartItem = new Cart
                {
                    AlbumId = albumId,
                    CartId = cartId,
                    Count = 1,
                    DateCreated = DateTime.UtcNow
                };
                await _cartRepository.AddCartItemAsync(cartItem);
            }
            else
            {
                cartItem.Count++;
                await _cartRepository.UpdateCartItemAsync(cartItem);
            }
        }

        public async Task<int> RemoveFromCartAsync(string cartId, int recordId)
        {
            var cartItem = await _cartRepository.GetCartItemByRecordIdAsync(recordId);

            int itemCount = 0;

            if (cartItem != null && cartItem.CartId == cartId)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                    await _cartRepository.UpdateCartItemAsync(cartItem);
                }
                else
                {
                    await _cartRepository.RemoveCartItemAsync(cartItem);
                }
            }

            return itemCount;
        }

        public async Task<int> GetCartCountAsync(string cartId)
        {
            return await _cartRepository.GetCartCountAsync(cartId);
        }

        public async Task<decimal> GetCartTotalAsync(string cartId)
        {
            return await _cartRepository.GetCartTotalAsync(cartId);
        }

        public async Task MigrateCartAsync(string oldCartId, string newCartId)
        {
            await _cartRepository.MigrateCartAsync(oldCartId, newCartId);
        }

        public async Task EmptyCartAsync(string cartId)
        {
            await _cartRepository.EmptyCartAsync(cartId);
        }

        public async Task<int> CreateOrderAsync(string cartId, Order order)
        {
            decimal orderTotal = 0;
            var cartItems = await _cartRepository.GetCartItemsAsync(cartId);

            foreach (var item in cartItems)
            {
                var album = await _albumRepository.GetByIdAsync(item.AlbumId);
                if (album != null)
                {
                    var orderDetail = new OrderDetail
                    {
                        AlbumId = item.AlbumId,
                        OrderId = order.OrderId,
                        UnitPrice = album.Price,
                        Quantity = item.Count
                    };
                    orderTotal += item.Count * album.Price;
                    order.OrderDetails.Add(orderDetail);
                }
            }

            order.Total = orderTotal;
            await _orderRepository.SaveChangesAsync();
            await EmptyCartAsync(cartId);

            return order.OrderId;
        }
    }
}
