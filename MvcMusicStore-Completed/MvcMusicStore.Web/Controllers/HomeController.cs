using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Domain.Entities;

namespace MvcMusicStore.Web.Controllers
{
    /// <summary>
    /// Home controller - displays top-selling albums on the home page.
    /// Migrated from legacy HomeController using System.Web.Mvc.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IAlbumService _albumService;

        public HomeController(IAlbumService albumService)
        {
            _albumService = albumService;
        }

        // GET: /Home/
        public async Task<IActionResult> Index()
        {
            var albums = await _albumService.GetTopSellingAlbumsAsync(5);
            return View(albums);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
