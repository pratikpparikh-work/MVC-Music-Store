using Microsoft.AspNetCore.Http;
using MvcMusicStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcMusicStore.Models
{
    /// <summary>
    /// Shopping cart model using ASP.NET Core session and IHttpContextAccessor.
    /// </summary>
    public partial class ShoppingCart
    {
        private readonly ApplicationDbContext _storeDB;
        private readonly HttpContext _httpContext;

        public const string CartSessionKey = "CartId";

        private string ShoppingCartId { get; set; } = string.Empty;

        private ShoppingCart(HttpContext httpContext, ApplicationDbContext storeDB)
        {
            _httpContext = httpContext;
            _storeDB = storeDB;
            ShoppingCartId = GetCartId();
        }

        /// <summary>
        /// Creates a ShoppingCart instance from the current HttpContext.
        /// </summary>
        public static ShoppingCart GetCart(HttpContext context, ApplicationDbContext storeDB)
        {
            return new ShoppingCart(context, storeDB);
        }

        public void AddToCart(Album album)
        {
            var cartItem = _storeDB.Carts.SingleOrDefault(
                c => c.CartId == ShoppingCartId && c.AlbumId == album.AlbumId);

            if (cartItem == null)
            {
                cartItem = new Cart
                {
                    AlbumId = album.AlbumId,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                _storeDB.Carts.Add(cartItem);
            }
            else
            {
                cartItem.Count++;
            }

            _storeDB.SaveChanges();
        }

        public int RemoveFromCart(int id)
        {
            var cartItem = _storeDB.Carts.Single(
                cart => cart.CartId == ShoppingCartId && cart.RecordId == id);

            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    _storeDB.Carts.Remove(cartItem);
                }

                _storeDB.SaveChanges();
            }

            return itemCount;
        }

        public void EmptyCart()
        {
            var cartItems = _storeDB.Carts.Where(cart => cart.CartId == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                _storeDB.Carts.Remove(cartItem);
            }

            _storeDB.SaveChanges();
        }

        public List<Cart> GetCartItems()
        {
            return _storeDB.Carts.Where(cart => cart.CartId == ShoppingCartId).ToList();
        }

        public int GetCount()
        {
            int? count = (from cartItems in _storeDB.Carts
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Count).Sum();

            return count ?? 0;
        }

        public decimal GetTotal()
        {
            decimal? total = (from cartItems in _storeDB.Carts
                              where cartItems.CartId == ShoppingCartId
                              select (int?)cartItems.Count * cartItems.Album!.Price).Sum();
            return total ?? decimal.Zero;
        }

        public int CreateOrder(Order order)
        {
            decimal orderTotal = 0;

            var cartItems = GetCartItems();

            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    AlbumId = item.AlbumId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Album!.Price,
                    Quantity = item.Count
                };

                orderTotal += (item.Count * item.Album.Price);

                _storeDB.OrderDetails.Add(orderDetail);
            }

            order.Total = orderTotal;

            _storeDB.SaveChanges();

            EmptyCart();

            return order.OrderId;
        }

        /// <summary>
        /// Gets or creates the cart ID from the ASP.NET Core session.
        /// </summary>
        public string GetCartId()
        {
            var cartId = _httpContext.Session.GetString(CartSessionKey);

            if (cartId == null)
            {
                if (!string.IsNullOrWhiteSpace(_httpContext.User?.Identity?.Name))
                {
                    cartId = _httpContext.User.Identity.Name;
                }
                else
                {
                    cartId = Guid.NewGuid().ToString();
                }

                _httpContext.Session.SetString(CartSessionKey, cartId);
            }

            return cartId;
        }

        /// <summary>
        /// Migrates the shopping cart to be associated with the logged-in user.
        /// </summary>
        public void MigrateCart(string userName)
        {
            var shoppingCart = _storeDB.Carts.Where(c => c.CartId == ShoppingCartId);

            foreach (Cart item in shoppingCart)
            {
                item.CartId = userName;
            }
            _storeDB.SaveChanges();
        }
    }
}
