using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokApp.Areas.Manage.ViewModels;
using PustokApp.Models;
using System.Security.Claims;

namespace PustokApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class AccountController 
        (UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager
        )
        : Controller

    {
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginVm adminLoginVm)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Username or Password is incorrect");
                return View();
            }
            var user = await userManager.FindByNameAsync(adminLoginVm.Username);
            if(user == null)
            {
                ModelState.AddModelError("", "Username or Password is incorrect");
                return View();
            }
            var result = await userManager.CheckPasswordAsync(user, adminLoginVm.Password);
            if(!result)
            {
                ModelState.AddModelError("", "Username or Password is incorrect");
                return View();
            }
            await signInManager.SignInAsync(user, false);

            return RedirectToAction("Index", "Dashboard");
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        public async Task<IActionResult> CreateAdmin()
        {
            AppUser appUser = new()
            {
                UserName = "_Admin",
                FullName = "Super Admin",
                Email = "admin@gmail.com"
            };
            var result = await userManager.CreateAsync(appUser, "_Admin123" );
            if(!result.Succeeded)
            {
               return Json(result.Errors);
            }
            await userManager.AddToRoleAsync(appUser, "Admin");
            return Json(result);
        }
        [Authorize]
        public async Task<IActionResult> UserProfile()
        {
            //var user = await userManager.FindByNameAsync(User.Identity.Name);
            //var user = await userManager.GetUserIdAsync(User);
            var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return Json(user);
        }
    }
}
