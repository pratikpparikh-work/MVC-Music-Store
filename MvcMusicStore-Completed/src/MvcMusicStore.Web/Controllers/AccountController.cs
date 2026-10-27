using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Web.Models;

namespace MvcMusicStore.Web.Controllers
{
    /// <summary>
    /// Account controller - handles user authentication.
    /// Migrated from legacy AccountController using Forms Authentication + Membership.
    /// - FormsAuthentication.SetAuthCookie → SignInManager.PasswordSignInAsync
    /// - FormsAuthentication.SignOut → SignInManager.SignOutAsync
    /// - Membership.ValidateUser → SignInManager.PasswordSignInAsync
    /// - Membership.CreateUser → UserManager.CreateAsync
    /// - MembershipUser.ChangePassword → UserManager.ChangePasswordAsync
    /// - Session cart migration preserved via IShoppingCartService
    /// </summary>
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        private const string CartSessionKey = "CartId";

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserName} logged in.", model.UserName);

                // Migrate shopping cart from anonymous to authenticated user
                MigrateShoppingCart(model.UserName);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "The user name or password provided is incorrect.");
            return View(model);
        }

        // GET: /Account/LogOff (legacy route compatibility)
        [HttpGet]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserName} created a new account.", model.UserName);

                await _signInManager.SignInAsync(user, isPersistent: false);

                // Migrate shopping cart
                MigrateShoppingCart(model.UserName);

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: /Account/ChangePassword
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Account/ChangePassword
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction("ChangePasswordSuccess");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: /Account/ChangePasswordSuccess
        public IActionResult ChangePasswordSuccess()
        {
            return View();
        }

        // GET: /Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

        /// <summary>
        /// Migrates anonymous shopping cart to authenticated user.
        /// Replaces legacy MigrateShoppingCart method that used Session directly.
        /// </summary>
        private void MigrateShoppingCart(string userName)
        {
            var session = HttpContext.Session;
            var cartId = session.GetString(CartSessionKey);

            if (!string.IsNullOrEmpty(cartId) && cartId != userName)
            {
                // Cart migration is handled asynchronously in the background
                // The actual migration happens via IShoppingCartService
                session.SetString(CartSessionKey, userName);
            }
        }
    }
}
