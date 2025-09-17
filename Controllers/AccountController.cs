using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Demo.Context;
using Demo.Helpers;
using Demo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.Controllers
{
    public class AccountController : Controller
    {
        private readonly MyDBContext _context;

        public AccountController(MyDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var account = _context.Accounts
                .Include(a => a.Role)   // include role if you need role claims
                .FirstOrDefault(a => a.Username == username);

            if (account == null)
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }

            // Re-hash the provided password using the stored salt
            var hash = PasswordHelper.HashPassword(password, account.Salt);

            if (hash != account.PasswordHash)
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Username),
                new Claim(ClaimTypes.Role, account.Role.RoleName)
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookieAuth", principal);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login", "Account");
        }
    }
}
