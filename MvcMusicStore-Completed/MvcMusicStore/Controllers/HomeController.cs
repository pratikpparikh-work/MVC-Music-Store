using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Data;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _storeDB;

        public HomeController(ApplicationDbContext storeDB)
        {
            _storeDB = storeDB;
        }

        // GET: /Home/
        public async Task<IActionResult> Index()
        {
            var albums = await GetTopSellingAlbumsAsync(5);
            return View(albums);
        }

        private async Task<List<Album>> GetTopSellingAlbumsAsync(int count)
        {
            return await _storeDB.Albums
                .OrderByDescending(a => a.OrderDetails.Count())
                .Take(count)
                .ToListAsync();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
