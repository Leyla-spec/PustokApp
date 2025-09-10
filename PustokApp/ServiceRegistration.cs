using Microsoft.AspNetCore.Identity;
using PustokApp.Data;
using PustokApp.Models;
using PustokApp.Services;
using Microsoft.EntityFrameworkCore;
namespace PustokApp
{
    public static class ServiceRegistration
    {
        public static void AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {

            // Add services to the container.
            services.AddControllersWithViews();
            services.AddDbContext<PustokDbContex>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString("Server=.\\SQLEXPRESS;Database=PustokApp;Trusted_Connection=True;TrustServerCertificate=True;")));
            services.AddScoped<LayoutService>();
            services.AddScoped<EmailService>();
            services.AddSession(opt =>
            {
                opt.IdleTimeout = TimeSpan.FromMinutes(20);
            });
            services.AddIdentity<AppUser, IdentityRole>(opt =>
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
                opt.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<PustokDbContex>()
                .AddDefaultTokenProviders();

        }
    }
}
