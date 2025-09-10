using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;
using AutoMapper;
using PustokApp.ViewModels;
using Microsoft.AspNet.Identity;
using PustokApp.Models;


namespace PustokApp.Controllers
{
    public class BookController
        (PustokDbContex pustokDbContex,
        IMapper mapper, 
        UserManager<AppUser> userManager
        )
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
            BookDetailVm bookDetailVm = new BookDetailVm
            {
                Book = book,
                RelatedBooks = pustokDbContex.books
                .Where(b => b.GenreId == book.GenreId && b.Id != book.Id)
                .Include(b => b.BookImages)
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookTags)
                .ThenInclude(bt => bt.Tag)
                .ToList()
            };
            bookDetailVm.AvgRate = pustokDbContex.BookComments
                .Where(bc => bc.BookId == book.Id && bc.Status == Models.CommentStatus.Approved)
                .Count() == 0 ? (int)pustokDbContex.BookComments
                .Where(bc => bc.BookId == book.Id && bc.Status == Models.CommentStatus.Approved)
                .Average(bc => bc.Rate) : 0;
            var users = userManager.Users.ToList();
            return View(bookDetailVm);
        }

    }
}
