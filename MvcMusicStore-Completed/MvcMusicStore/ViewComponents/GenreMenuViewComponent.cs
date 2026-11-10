using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Data;
using MvcMusicStore.Models;

namespace MvcMusicStore.ViewComponents
{
    /// <summary>
    /// ViewComponent replacing the legacy [ChildActionOnly] GenreMenu action.
    /// </summary>
    public class GenreMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _storeDB;

        public GenreMenuViewComponent(ApplicationDbContext storeDB)
        {
            _storeDB = storeDB;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var genres = await _storeDB.Genres.ToListAsync();
            return View("Default", genres);
        }
    }
}
