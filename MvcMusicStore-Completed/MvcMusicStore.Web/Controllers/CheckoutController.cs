using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Domain.Interfaces;

namespace MvcMusicStore.Web.Controllers
{
    /// <summary>
    /// Checkout controller - handles order placement.
    /// Migrated from legacy CheckoutController.
    /// - [Authorize] works the same in ASP.NET Core
    /// - TryUpdateModel replaced with model binding
    /// - FormCollection replaced with strongly-typed model
    /// </summary>
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly IShoppingCartService _cartService;
        private readonly IOrderRepository _orderRepository;
        private const string PromoCode = "FREE";

        public CheckoutController(IShoppingCartService cartService, IOrderRepository orderRepository)
        {
            _cartService = cartService;
            _orderRepository = orderRepository;
        }

        // GET: /Checkout/AddressAndPayment
        public IActionResult AddressAndPayment()
        {
            return View();
        }

        // POST: /Checkout/AddressAndPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddressAndPayment(Order order, string promoCode)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }

            if (!string.Equals(promoCode, PromoCode, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("promoCode", "Invalid promo code.");
                return View(order);
            }

            try
            {
                order.Username = User.Identity!.Name!;
                order.OrderDate = DateTime.UtcNow;

                // Save order
                var savedOrder = await _orderRepository.AddAsync(order);

                // Get cart ID from session
                var cartId = HttpContext.Session.GetString(ShoppingCartController.CartSessionKey) ?? string.Empty;

                // Process the order
                await _cartService.CreateOrderAsync(cartId, savedOrder);

                return RedirectToAction("Complete", new { id = savedOrder.OrderId });
            }
            catch
            {
                return View(order);
            }
        }

        // GET: /Checkout/Complete
        public async Task<IActionResult> Complete(int id)
        {
            bool isValid = await _orderRepository.IsOrderOwnedByUserAsync(id, User.Identity!.Name!);

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
