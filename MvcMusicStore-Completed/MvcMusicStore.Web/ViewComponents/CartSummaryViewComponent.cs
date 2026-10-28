using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Web.Controllers;

namespace MvcMusicStore.Web.ViewComponents
{
    /// <summary>
    /// ViewComponent that displays the cart item count in the navigation bar.
    /// Replaces the legacy [ChildActionOnly] CartSummary action in ShoppingCartController.
    /// Html.RenderAction("CartSummary", "ShoppingCart") is replaced with
    /// @await Component.InvokeAsync("CartSummary") in _Layout.cshtml.
    /// </summary>
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly IShoppingCartService _cartService;

        public CartSummaryViewComponent(IShoppingCartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cartId = HttpContext.Session.GetString(ShoppingCartController.CartSessionKey) ?? string.Empty;
            int cartCount = 0;

            if (!string.IsNullOrEmpty(cartId))
            {
                cartCount = await _cartService.GetCartCountAsync(cartId);
            }

            return View(cartCount);
        }
    }
}
