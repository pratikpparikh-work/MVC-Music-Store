using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Application.Interfaces;

namespace MvcMusicStore.Web.Controllers
{
    /// <summary>
    /// Store controller - browse genres and album details.
    /// Migrated from legacy StoreController.
    /// ChildActionOnly actions replaced with ViewComponents in ASP.NET Core.
    /// </summary>
    public class StoreController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly IAlbumService _albumService;
        private readonly ILogger<StoreController> _logger;

        public StoreController(IGenreService genreService, IAlbumService albumService, ILogger<StoreController> logger)
        {
            _genreService = genreService;
            _albumService = albumService;
            _logger = logger;
        }

        // GET: /Store/
        public async Task<IActionResult> Index()
        {
            var genres = await _genreService.GetAllGenresAsync();
            return View(genres);
        }

        // GET: /Store/Browse?genre=Disco
        public async Task<IActionResult> Browse(string genre)
        {
            var genreModel = await _genreService.GetGenreWithAlbumsAsync(genre);
            if (genreModel == null)
            {
                return NotFound();
            }
            return View(genreModel);
        }

        // GET: /Store/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var album = await _albumService.GetAlbumByIdAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            return View(album);
        }

        // Replaces [ChildActionOnly] GenreMenu - now a ViewComponent
        // See: Views/Shared/Components/GenreMenu/Default.cshtml
    }
}
