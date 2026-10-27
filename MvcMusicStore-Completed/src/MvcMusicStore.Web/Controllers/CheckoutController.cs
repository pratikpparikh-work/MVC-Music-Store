using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Domain.Entities;

namespace MvcMusicStore.Web.Controllers
{
    /// <summary>
    /// Checkout controller - handles order placement.
    /// Migrated from legacy CheckoutController.
    /// - [Authorize] attribute works the same in ASP.NET Core
    /// - TryUpdateModel replaced with model binding + validation
    /// - FormCollection replaced with strongly-typed model binding
    /// - User.Identity.Name works the same in ASP.NET Core
    /// </summary>
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly IShoppingCartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CheckoutController> _logger;

        private const string CartSessionKey = "CartId";
        private const string PromoCode = "FREE";

        public CheckoutController(
            IShoppingCartService cartService,
            IOrderService orderService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CheckoutController> logger)
        {
            _cartService = cartService;
            _orderService = orderService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        // GET: /Checkout/AddressAndPayment
        public IActionResult AddressAndPayment()
        {
            return View();
        }

        // POST: /Checkout/AddressAndPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddressAndPayment(Order order, string PromoCode)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }

            if (!string.Equals(PromoCode, CheckoutController.PromoCode, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("PromoCode", "Invalid promo code.");
                return View(order);
            }

            try
            {
                order.Username = User.Identity?.Name ?? string.Empty;
                order.OrderDate = DateTime.UtcNow;

                // Save order via service
                var cartId = GetCartId();
                var orderId = await _cartService.CreateOrderAsync(cartId, order);

                return RedirectToAction("Complete", new { id = orderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for user {Username}", User.Identity?.Name);
                return View(order);
            }
        }

        // GET: /Checkout/Complete
        public async Task<IActionResult> Complete(int id)
        {
            bool isValid = await _orderService.ValidateOrderOwnerAsync(id, User.Identity?.Name ?? string.Empty);

            if (isValid)
            {
                return View(id);
            }
            else
            {
                return View("Error");
            }
        }

        private string GetCartId()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            return session?.GetString(CartSessionKey) ?? string.Empty;
        }
    }
}
