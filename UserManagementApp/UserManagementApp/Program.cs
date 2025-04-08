using Microsoft.EntityFrameworkCore;
using UserManagementApp.Data;
using UserManagementApp.Filters;
using BCrypt.Net;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
));

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<AuthFilter>();
});

builder.Services.AddScoped<AuthFilter>();

var app = builder.Build();

// Temporary route to fix password hashing (Remove after use!)
app.MapGet("/update-passwords", async (ApplicationDbContext context) =>
{
    var users = await context.Users.ToListAsync();
    foreach (var user in users)
    {
        if (!user.Password.StartsWith("$2a$")) // Check if not already hashed
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        }
    }
    await context.SaveChangesAsync();
    return "Passwords updated successfully!";
});

// Middleware pipeline
app.UseSession();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();