using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Application.Interfaces;

namespace MvcMusicStore.Web.ViewComponents
{
    /// <summary>
    /// GenreMenu ViewComponent - replaces legacy [ChildActionOnly] GenreMenu action.
    /// In legacy code: Html.RenderAction("GenreMenu", "Store")
    /// In .NET 8: @await Component.InvokeAsync("GenreMenu")
    /// </summary>
    public class GenreMenuViewComponent : ViewComponent
    {
        private readonly IGenreService _genreService;

        public GenreMenuViewComponent(IGenreService genreService)
        {
            _genreService = genreService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var genres = await _genreService.GetAllGenresAsync();
            return View(genres);
        }
    }

    /// <summary>
    /// CartSummary ViewComponent - replaces legacy [ChildActionOnly] CartSummary action.
    /// In legacy code: Html.RenderAction("CartSummary", "ShoppingCart")
    /// In .NET 8: @await Component.InvokeAsync("CartSummary")
    /// </summary>
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly IShoppingCartService _cartService;

        private const string CartSessionKey = "CartId";

        public CartSummaryViewComponent(IShoppingCartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cartId = HttpContext.Session.GetString(CartSessionKey) ?? string.Empty;
            var cartCount = await _cartService.GetCartCountAsync(cartId);
            ViewData["CartCount"] = cartCount;
            return View();
        }
    }
}
