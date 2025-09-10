namespace PustokApp.Extensions
{
    public class ServiceRegistration
{
    public static void AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {

        // Add services to the container.
        services.Services.AddControllersWithViews();
        services.Services.AddDbContext<PustokDbContex>(opt =>
        opt.UseSqlServer(configuration.GetConnectionString("Server=.\\SQLEXPRESS;Database=PustokApp;Trusted_Connection=True;TrustServerCertificate=True;")));
        services.Services.AddScoped<LayoutService>();
        services.Services.AddScoped<EmailService>();
        services.Services.AddSession(opt =>
        {
            opt.IdleTimeout = TimeSpan.FromMinutes(20);
        });
        services.Services.AddIdentity<AppUser, IdentityRole>(opt =>
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
