using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Data;
using MvcMusicStore.Models;
using MvcMusicStore.ViewModels;
using System.Net;

namespace MvcMusicStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _storeDB;
        private readonly ShoppingCart _cart;

        public ShoppingCartController(ApplicationDbContext storeDB, ShoppingCart cart)
        {
            _storeDB = storeDB;
            _cart = cart;
        }

        // GET: /ShoppingCart/
        public IActionResult Index()
        {
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = _cart.GetCartItems(),
                CartTotal = _cart.GetTotal()
            };

            return View(viewModel);
        }

        // GET: /Store/AddToCart/5
        public async Task<IActionResult> AddToCart(int id)
        {
            var addedAlbum = await _storeDB.Albums
                .SingleOrDefaultAsync(album => album.AlbumId == id);

            if (addedAlbum == null)
            {
                return NotFound();
            }

            _cart.AddToCart(addedAlbum);

            return RedirectToAction("Index");
        }

        // AJAX: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _storeDB.Carts
                .Include(c => c.Album)
                .SingleOrDefaultAsync(item => item.RecordId == id);

            if (cartItem == null)
            {
                return NotFound();
            }

            string albumName = cartItem.Album?.Title ?? string.Empty;

            int itemCount = _cart.RemoveFromCart(id);

            var results = new ShoppingCartRemoveViewModel
            {
                Message = WebUtility.HtmlEncode(albumName) + " has been removed from your shopping cart.",
                CartTotal = _cart.GetTotal(),
                CartCount = _cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };

            return Json(results);
        }
    }
}
