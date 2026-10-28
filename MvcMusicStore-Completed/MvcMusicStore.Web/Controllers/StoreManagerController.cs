using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Domain.Entities;

namespace MvcMusicStore.Web.Controllers
{
    /// <summary>
    /// Store manager controller - CRUD operations for albums (admin only).
    /// Migrated from legacy StoreManagerController.
    /// - ViewResult return types replaced with IActionResult
    /// - db.Entry(album).State = EntityState.Modified replaced with service call
    /// - Dispose pattern replaced with DI scoped lifetime
    /// </summary>
    [Authorize(Roles = "Administrator")]
    public class StoreManagerController : Controller
    {
        private readonly IAlbumService _albumService;
        private readonly IGenreService _genreService;
        private readonly IArtistService _artistService;

        public StoreManagerController(
            IAlbumService albumService,
            IGenreService genreService,
            IArtistService artistService)
        {
            _albumService = albumService;
            _genreService = genreService;
            _artistService = artistService;
        }

        // GET: /StoreManager/
        public async Task<IActionResult> Index()
        {
            var albums = await _albumService.GetAllAlbumsAsync();
            return View(albums);
        }

        // GET: /StoreManager/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var album = await _albumService.GetAlbumByIdAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            return View(album);
        }

        // GET: /StoreManager/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropDownsAsync();
            return View();
        }

        // POST: /StoreManager/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Album album)
        {
            if (ModelState.IsValid)
            {
                await _albumService.CreateAlbumAsync(album);
                return RedirectToAction("Index");
            }

            await PopulateDropDownsAsync(album.GenreId, album.ArtistId);
            return View(album);
        }

        // GET: /StoreManager/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var album = await _albumService.GetAlbumByIdAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            await PopulateDropDownsAsync(album.GenreId, album.ArtistId);
            return View(album);
        }

        // POST: /StoreManager/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Album album)
        {
            if (ModelState.IsValid)
            {
                await _albumService.UpdateAlbumAsync(album);
                return RedirectToAction("Index");
            }
            await PopulateDropDownsAsync(album.GenreId, album.ArtistId);
            return View(album);
        }

        // GET: /StoreManager/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var album = await _albumService.GetAlbumByIdAsync(id);
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
            await _albumService.DeleteAlbumAsync(id);
            return RedirectToAction("Index");
        }

        private async Task PopulateDropDownsAsync(int? selectedGenreId = null, int? selectedArtistId = null)
        {
            var genres = await _genreService.GetAllGenresAsync();
            var artists = await _artistService.GetAllArtistsAsync();
            ViewBag.GenreId = new SelectList(genres, "GenreId", "Name", selectedGenreId);
            ViewBag.ArtistId = new SelectList(artists, "ArtistId", "Name", selectedArtistId);
        }
    }
}
