using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;

namespace PustokApp.Controllers
{
    public class BookController
        (PustokDbContex pustokDbContex)
        : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detail(int? id)
        {
            if (id == null)
            return NotFound();
            var book = pustokDbContex.books.
                Include(b => b.BookImages).
                Include(b => b.Author).
                Include(b => b.Genre).
                Include(b => b.BookTags).
                ThenInclude(bt => bt.Tag).
                FirstOrDefault(b => b.Id == id);
            if (book is null)
                return NotFound();
            return View(book);
        }

    }
}
