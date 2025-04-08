using Microsoft.AspNetCore.Mvc;
using UserManagementApp.Data;
using UserManagementApp.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace UserManagementApp.Controllers
{
    public class AccountController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // Registration page
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(user);
        }

        // Login page
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || user.Status == "Blocked")
            {
                ModelState.AddModelError("", "Invalid login or account blocked.");
                return View();
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (!isPasswordValid)
            {
                ModelState.AddModelError("", "Invalid credentials.");
                return View();
            }

            user.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            return RedirectToAction("Index", "Users");
        }
    }
}