using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Data;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _storeDB;

        public StoreController(ApplicationDbContext storeDB)
        {
            _storeDB = storeDB;
        }

        // GET: /Store/
        public async Task<IActionResult> Index()
        {
            var genres = await _storeDB.Genres.ToListAsync();
            return View(genres);
        }

        // GET: /Store/Browse?genre=Disco
        public async Task<IActionResult> Browse(string genre)
        {
            var genreModel = await _storeDB.Genres
                .Include(g => g.Albums)
                .SingleOrDefaultAsync(g => g.Name == genre);

            if (genreModel == null)
            {
                return NotFound();
            }

            return View(genreModel);
        }

        // GET: /Store/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var album = await _storeDB.Albums.FindAsync(id);

            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }
    }
}
