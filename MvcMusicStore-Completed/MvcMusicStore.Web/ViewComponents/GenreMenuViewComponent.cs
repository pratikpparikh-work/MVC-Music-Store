using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Application.Interfaces;

namespace MvcMusicStore.Web.ViewComponents
{
    /// <summary>
    /// ViewComponent that displays the genre navigation menu.
    /// Replaces the legacy [ChildActionOnly] GenreMenu action in StoreController.
    /// Html.RenderAction("GenreMenu", "Store") is replaced with
    /// @await Component.InvokeAsync("GenreMenu") in _Layout.cshtml.
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
}
