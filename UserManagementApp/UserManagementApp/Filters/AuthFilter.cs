using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using UserManagementApp.Data;
using UserManagementApp.Models;

namespace UserManagementApp.Filters
{
    public class AuthFilter(ApplicationDbContext context) : IActionFilter
    {
        private readonly ApplicationDbContext _context = context;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerName = context.Controller.GetType().Name;
            if (controllerName == "AccountController") return;

            var userId = context.HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            var user = _context.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.UserId == int.Parse(userId));

            if (user == null || user.Status == "Blocked")
            {
                context.HttpContext.Session.Remove("UserId");
                context.Result = new RedirectToActionResult("Login", "Account", new { message = "Your account is blocked" });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}