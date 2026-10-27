using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Application.DTOs;
using MvcMusicStore.Application.Interfaces;

namespace MvcMusicStore.Web.Controllers
{
    /// <summary>
    /// Shopping cart controller.
    /// Migrated from legacy ShoppingCartController.
    /// - Session access via IHttpContextAccessor (replaces HttpContext.Session directly)
    /// - [ChildActionOnly] CartSummary replaced with ViewComponent
    /// - Server.HtmlEncode replaced with HtmlEncoder
    /// - Json() returns IActionResult (same API in Core)
    /// </summary>
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _cartService;
        private readonly IAlbumService _albumService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ShoppingCartController> _logger;

        private const string CartSessionKey = "CartId";

        public ShoppingCartController(
            IShoppingCartService cartService,
            IAlbumService albumService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ShoppingCartController> logger)
        {
            _cartService = cartService;
            _albumService = albumService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        // GET: /ShoppingCart/
        public async Task<IActionResult> Index()
        {
            var cartId = GetCartId();
            var cart = await _cartService.GetCartAsync(cartId);
            return View(cart);
        }

        // GET: /ShoppingCart/AddToCart/5
        public async Task<IActionResult> AddToCart(int id)
        {
            var album = await _albumService.GetAlbumByIdAsync(id);
            if (album == null)
            {
                return NotFound();
            }

            var cartId = GetCartId();
            await _cartService.AddToCartAsync(cartId, id);

            return RedirectToAction("Index");
        }

        // AJAX POST: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartId = GetCartId();

            // Get album name before removing
            var albumTitle = await _cartService.GetAlbumTitleByCartRecordIdAsync(cartId, id);
            var safeAlbumTitle = System.Net.WebUtility.HtmlEncode(albumTitle ?? "Item");

            int itemCount = await _cartService.RemoveFromCartAsync(cartId, id);

            var results = new CartRemoveDto
            {
                Message = $"{safeAlbumTitle} has been removed from your shopping cart.",
                CartTotal = await _cartService.GetCartTotalAsync(cartId),
                CartCount = await _cartService.GetCartCountAsync(cartId),
                ItemCount = itemCount,
                DeleteId = id
            };

            return Json(results);
        }

        /// <summary>
        /// Gets or creates a cart ID from session.
        /// Replaces legacy ShoppingCart.GetCartId(HttpContextBase context).
        /// </summary>
        private string GetCartId()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return Guid.NewGuid().ToString();

            if (session.GetString(CartSessionKey) == null)
            {
                if (!string.IsNullOrWhiteSpace(User.Identity?.Name))
                {
                    session.SetString(CartSessionKey, User.Identity.Name);
                }
                else
                {
                    session.SetString(CartSessionKey, Guid.NewGuid().ToString());
                }
            }

            return session.GetString(CartSessionKey) ?? Guid.NewGuid().ToString();
        }
    }
}
