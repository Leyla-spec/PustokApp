using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PustokApp;
using PustokApp.Data;
using PustokApp.Models;
using PustokApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppServices(builder.Configuration);
// Add services to the container.


var app = builder.Build();
app.UseSession();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=dashboard}/{action=Index}/{id?}"
          );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
