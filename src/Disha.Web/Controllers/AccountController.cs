using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Disha.Web.Data;
using Disha.Web.Models.Entities;
using Disha.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Disha.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly DishaDbContext _context;

        public AccountController(DishaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string usernameLower = model.Username.Trim().ToLowerInvariant();
            string emailLower = model.Email.Trim().ToLowerInvariant();

            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == usernameLower))
            {
                ModelState.AddModelError("Username", "That username is already taken.");
                return View(model);
            }

            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == emailLower))
            {
                ModelState.AddModelError("Email", "That email address is already registered.");
                return View(model);
            }

            var newUser = new User
            {
                Username = model.Username.Trim(),
                Email = model.Email.Trim(),
                FullName = model.FullName.Trim(),
                Role = "student",
                EducationLevel = null,
                PriorComputerExposure = "none",
                State = null,
                LanguagePref = "en",
                CreatedAt = DateTime.UtcNow
            };

            var hasher = new PasswordHasher<User>();
            newUser.PasswordHash = hasher.HashPassword(newUser, model.Password);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Sign in immediately after registration
            await SignInUserAsync(newUser);

            return RedirectToAction("Index", "Onboarding");
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            // Sanitize returnUrl to prevent redirect loops
            if (!string.IsNullOrEmpty(returnUrl) && (returnUrl.Contains("/Account/", StringComparison.OrdinalIgnoreCase)))
            {
                returnUrl = null;
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(returnUrl) && (returnUrl.Contains("/Account/", StringComparison.OrdinalIgnoreCase)))
            {
                returnUrl = null;
            }
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string input = model.UsernameOrEmail.Trim().ToLowerInvariant();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == input || u.Email.ToLower() == input);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Username or password doesn't match.");
                return View(model);
            }

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Username or password doesn't match.");
                return View(model);
            }

            await SignInUserAsync(user);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUserAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("FullName", user.FullName ?? user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}
