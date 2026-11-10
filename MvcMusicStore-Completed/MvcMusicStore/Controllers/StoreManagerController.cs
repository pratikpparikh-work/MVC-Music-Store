using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Data;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class StoreManagerController : Controller
    {
        private readonly ApplicationDbContext _db;

        public StoreManagerController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /StoreManager/
        public async Task<IActionResult> Index()
        {
            var albums = await _db.Albums
                .Include(a => a.Genre)
                .Include(a => a.Artist)
                .ToListAsync();
            return View(albums);
        }

        // GET: /StoreManager/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var album = await _db.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            return View(album);
        }

        // GET: /StoreManager/Create
        public IActionResult Create()
        {
            ViewBag.GenreId = new SelectList(_db.Genres, "GenreId", "Name");
            ViewBag.ArtistId = new SelectList(_db.Artists, "ArtistId", "Name");
            return View();
        }

        // POST: /StoreManager/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("GenreId,ArtistId,Title,Price,AlbumArtUrl")] Album album)
        {
            if (ModelState.IsValid)
            {
                _db.Albums.Add(album);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.GenreId = new SelectList(_db.Genres, "GenreId", "Name", album.GenreId);
            ViewBag.ArtistId = new SelectList(_db.Artists, "ArtistId", "Name", album.ArtistId);
            return View(album);
        }

        // GET: /StoreManager/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var album = await _db.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            ViewBag.GenreId = new SelectList(_db.Genres, "GenreId", "Name", album.GenreId);
            ViewBag.ArtistId = new SelectList(_db.Artists, "ArtistId", "Name", album.ArtistId);
            return View(album);
        }

        // POST: /StoreManager/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            [Bind("AlbumId,GenreId,ArtistId,Title,Price,AlbumArtUrl")] Album album)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(album).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.GenreId = new SelectList(_db.Genres, "GenreId", "Name", album.GenreId);
            ViewBag.ArtistId = new SelectList(_db.Artists, "ArtistId", "Name", album.ArtistId);
            return View(album);
        }

        // GET: /StoreManager/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var album = await _db.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            return View(album);
        }

        // POST: /StoreManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var album = await _db.Albums.FindAsync(id);
            if (album != null)
            {
                _db.Albums.Remove(album);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
