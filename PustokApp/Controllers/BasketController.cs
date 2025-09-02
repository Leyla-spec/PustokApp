using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;

namespace PustokApp.Controllers
{
    public class BasketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public void SetCookie()
        {
            string value = "Pustok";
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Append("CompanyName", value, cookieOptions);
        }
        public string GetCookie()
        {
            string value = Request.Cookies["CompanyName"];
            return value;
        }
        public IActionResult DeleteCookie()
        {
            Response.Cookies.Delete("CompanyName");
            return RedirectToAction("Index");
        }
        public IActionResult SessionSet()
        {
            HttpContext.Session.SetString("CompanyName", "Pustok");
            return RedirectToAction("Index");
        }
    }
}
