using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;
using PustokApp.Models;
using PustokApp.ViewModels;

namespace PustokApp.Controllers
{
    public class HomeController
        (PustokDbContex pustokDbContex)
        : Controller
    {
        public IActionResult Index()
        {
            HomeVm homeVm =  new()
            {
                Sliders = pustokDbContex.Sliders.ToList(),
                FeaturedBooks = pustokDbContex.books.
                Include(b => b.Author).
                Where(b => b.IsFeatured).
                ToList(),
                NewBooks = pustokDbContex.books.
                Include(b => b.Author).
                Where(b => b.IsNew).
                ToList(),
                DiscountBooks = pustokDbContex.books.
                Include(b => b.Author).
                Where(b => b.DiscountPercentage > 0).
                ToList()
            };
            return View(homeVm);
        }
    }
}
