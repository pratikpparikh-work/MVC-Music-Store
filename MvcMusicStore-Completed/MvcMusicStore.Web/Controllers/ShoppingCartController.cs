using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Web.ViewModels;

namespace MvcMusicStore.Web.Controllers
{
    /// <summary>
    /// Shopping cart controller.
    /// Migrated from legacy ShoppingCartController.
    /// - HttpContext.Session replaces System.Web.HttpContext session
    /// - [ChildActionOnly] CartSummary replaced with ViewComponent
    /// - Server.HtmlEncode replaced with HtmlEncoder
    /// </summary>
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _cartService;
        private readonly IAlbumService _albumService;

        public const string CartSessionKey = "CartId";

        public ShoppingCartController(IShoppingCartService cartService, IAlbumService albumService)
        {
            _cartService = cartService;
            _albumService = albumService;
        }

        // GET: /ShoppingCart/
        public async Task<IActionResult> Index()
        {
            var cartId = GetCartId();
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = await _cartService.GetCartItemsAsync(cartId),
                CartTotal = await _cartService.GetCartTotalAsync(cartId)
            };
            return View(viewModel);
        }

        // GET: /ShoppingCart/AddToCart/5
        public async Task<IActionResult> AddToCart(int id)
        {
            var cartId = GetCartId();
            await _cartService.AddToCartAsync(cartId, id);
            return RedirectToAction("Index");
        }

        // POST: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartId = GetCartId();

            // Get album name for confirmation message
            var cartItem = (await _cartService.GetCartItemsAsync(cartId))
                .Find(c => c.RecordId == id);
            string albumName = cartItem?.Album?.Title ?? "Item";

            int itemCount = await _cartService.RemoveFromCartAsync(cartId, id);

            var results = new ShoppingCartRemoveViewModel
            {
                Message = System.Net.WebUtility.HtmlEncode(albumName) + " has been removed from your shopping cart.",
                CartTotal = await _cartService.GetCartTotalAsync(cartId),
                CartCount = await _cartService.GetCartCountAsync(cartId),
                ItemCount = itemCount,
                DeleteId = id
            };

            return Json(results);
        }

        /// <summary>
        /// Gets or creates a cart ID from the session.
        /// Replaces the legacy GetCartId(HttpContextBase context) method.
        /// </summary>
        private string GetCartId()
        {
            if (HttpContext.Session.GetString(CartSessionKey) == null)
            {
                if (!string.IsNullOrWhiteSpace(User.Identity?.Name))
                {
                    HttpContext.Session.SetString(CartSessionKey, User.Identity.Name);
                }
                else
                {
                    var tempCartId = Guid.NewGuid().ToString();
                    HttpContext.Session.SetString(CartSessionKey, tempCartId);
                }
            }
            return HttpContext.Session.GetString(CartSessionKey)!;
        }
    }
}
