using Microsoft.AspNetCore.Mvc;

namespace PustokApp.Areas.Manage.Controllers
{
[Area("Manage")]
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
