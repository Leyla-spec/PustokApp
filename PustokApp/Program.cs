using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PustokApp.Data;
using PustokApp.Models;
using PustokApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<PustokDbContex>(opt => 
opt.UseSqlServer("Server=.\\SQLEXPRESS;Database=PustokApp;Trusted_Connection=True;TrustServerCertificate=True;"
, sqlOptions => sqlOptions.CommandTimeout(180)
));
builder.Services.AddScoped<LayoutService>();
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(20);
});
builder.Services.AddIdentity<AppUser, IdentityRole>(opt=> 
{
    opt.Password.RequireNonAlphanumeric = true;
    opt.Password.RequiredLength = 4;
    opt.User.RequireUniqueEmail = true;
    opt.Password.RequireUppercase = true;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireDigit = true;
    opt.Lockout.MaxFailedAccessAttempts = 5;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    opt.Lockout.AllowedForNewUsers = true;
}).AddEntityFrameworkStores<PustokDbContex>().AddDefaultTokenProviders();

var app = builder.Build();
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=dashboard}/{action=Index}/{id?}"
          );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
