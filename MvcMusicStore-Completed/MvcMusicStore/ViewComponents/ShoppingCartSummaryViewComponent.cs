using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Models;

namespace MvcMusicStore.ViewComponents
{
    /// <summary>
    /// ViewComponent replacing the legacy [ChildActionOnly] CartSummary action.
    /// </summary>
    public class ShoppingCartSummaryViewComponent : ViewComponent
    {
        private readonly ShoppingCart _cart;

        public ShoppingCartSummaryViewComponent(ShoppingCart cart)
        {
            _cart = cart;
        }

        public IViewComponentResult Invoke()
        {
            ViewData["CartCount"] = _cart.GetCount();
            return View("Default");
        }
    }
}
