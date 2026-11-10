using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Data;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _storeDB;
        private readonly ShoppingCart _cart;
        private const string PromoCode = "FREE";

        public CheckoutController(ApplicationDbContext storeDB, ShoppingCart cart)
        {
            _storeDB = storeDB;
            _cart = cart;
        }

        // GET: /Checkout/AddressAndPayment
        public IActionResult AddressAndPayment()
        {
            return View();
        }

        // POST: /Checkout/AddressAndPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddressAndPayment(
            [Bind("FirstName,LastName,Address,City,State,PostalCode,Country,Phone,Email")] Order order,
            string? promoCode)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }

            try
            {
                if (!string.Equals(promoCode, PromoCode, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("PromoCode", "Invalid promo code.");
                    return View(order);
                }

                order.Username = User.Identity!.Name!;
                order.OrderDate = DateTime.Now;

                _storeDB.Orders.Add(order);
                await _storeDB.SaveChangesAsync();

                _cart.CreateOrder(order);

                return RedirectToAction("Complete", new { id = order.OrderId });
            }
            catch
            {
                return View(order);
            }
        }

        // GET: /Checkout/Complete
        public async Task<IActionResult> Complete(int id)
        {
            bool isValid = await _storeDB.Orders.AnyAsync(
                o => o.OrderId == id && o.Username == User.Identity!.Name);

            if (isValid)
            {
                return View(id);
            }
            else
            {
                return View("Error");
            }
        }
    }
}
