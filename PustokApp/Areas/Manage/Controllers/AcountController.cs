using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokApp.Models;

namespace PustokApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class AcountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        public async Task<IActionResult> CreateAdmin()
        {
            AppUser appUser = new()
            {
                UserName = "_Admin",
                FullName = "Super Admin"
            };
            var rresult = await userManager.CreateAsync(appUser, "_Admin" );
            return View();
        }
    }
}
