using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Domain.Entities;

namespace MvcMusicStore.Web.Controllers
{
    /// <summary>
    /// Store controller - browse genres, albums, and album details.
    /// Migrated from legacy StoreController.
    /// [ChildActionOnly] replaced with ViewComponents in ASP.NET Core.
    /// </summary>
    public class StoreController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly IAlbumService _albumService;

        public StoreController(IGenreService genreService, IAlbumService albumService)
        {
            _genreService = genreService;
            _albumService = albumService;
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
            var genreModel = await _genreService.GetGenreByNameAsync(genre);
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
    }
}
