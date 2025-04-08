using Microsoft.AspNetCore.Mvc;
using UserManagementApp.Data;
using UserManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace UserManagementApp.Controllers
{
    public class UsersController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // Display user list sorted by last login
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.LastLogin)
                .ToListAsync();
            return View(users);
        }

        // Handle block/unblock/delete actions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUsers([FromBody] UserActionRequest request)
        {
            var currentUserId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(currentUserId))
                return Json(new { redirectUrl = Url.Action("Login", "Account") });

            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == int.Parse(currentUserId));

            if (currentUser == null || currentUser.Status == "Blocked")
                return Json(new { redirectUrl = Url.Action("Login", "Account") });

            var users = await _context.Users
                .Where(u => request.UserIds.Contains(u.UserId))
                .ToListAsync();

            foreach (var user in users)
            {
                switch (request.Action)
                {
                    case "Block":
                        user.Status = "Blocked";
                        if (user.UserId == currentUser.UserId)
                            HttpContext.Session.Remove("UserId");
                        break;
                    case "Unblock":
                        user.Status = "Active";
                        break;
                    case "Delete":
                        _context.Users.Remove(user);
                        if (user.UserId == currentUser.UserId)
                            HttpContext.Session.Remove("UserId");
                        break;
                }
            }

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                redirect = request.UserIds.Contains(currentUser.UserId)
                    ? Url.Action("Login", "Account")
                    : null
            });
        }

        // Request model for user actions
        public class UserActionRequest
        {
            public required List<int> UserIds { get; set; }
            public required string Action { get; set; }
        }
    }
}