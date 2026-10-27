using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Application.Interfaces;

namespace MvcMusicStore.Web.Controllers
{
    /// <summary>
    /// Home controller - displays top-selling albums on the home page.
    /// Migrated from legacy HomeController using System.Web.Mvc.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IAlbumService _albumService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IAlbumService albumService, ILogger<HomeController> logger)
        {
            _albumService = albumService;
            _logger = logger;
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
