using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;
using PustokApp.Extension;
using PustokApp.Models.BookSlider;
using PustokApp.Extension;
using PustokApp.Models;

namespace PustokApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class BookController 
        (PustokDbContex pustokDbContex)
        : Controller
    {
        public IActionResult Index()
        {
            var books = pustokDbContex.books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .ToList();
            return View(books);
        }
        public IActionResult Delete(int id)
        {
            var book = pustokDbContex.books
                .Include( b => b.BookImages)
                .FirstOrDefault(b => b.Id==id);
            if (book == null) return NotFound();
            pustokDbContex.books.Remove(book);
            pustokDbContex.SaveChanges();
            FileManager.DeleteFile("image/products", book.MainImageUrl);
            FileManager.DeleteFile("image/products", book.HoverImageUrl);

            foreach (var item in book.BookImages)
            {
                FileManager.DeleteFile("image/products", item.ImageUrl);
            }

            return RedirectToAction("Index");
        }
        public IActionResult Cretae()
        {
            ViewBag.Authors = pustokDbContex.Authors.ToList();
            ViewBag.Genres = pustokDbContex.Genre.ToList();
            ViewBag.Tags = pustokDbContex.Tags.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create (Book book)
        {
            ViewBag.Authors = pustokDbContex.Authors.ToList();
            ViewBag.Genres = pustokDbContex.Genre.ToList();
            ViewBag.Tags = pustokDbContex.Tags.ToList();
            if (!ModelState.IsValid)
                return View();
            if(!pustokDbContex.Authors.Any(a=> a.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Author is not valid");
                return View();
            }
            if (!pustokDbContex.Genre.Any(g => g.Id == book.GenreId))
            {
                ModelState.AddModelError("GenreId", "Genre is not valid");
                return View();
            }

            foreach (var tagId in book.TagIds)
            {
               
                if (!pustokDbContex.Tags.Any(t => t.Id == tagId))
                {
                    ModelState.AddModelError("TagIds", "Tag is not valid");
                    return View(book);
                }
            }
            foreach (var tagId in book.TagIds)
            {
                var bookTag = new BookTag
                {
                    TagId = tagId,
                    BookId = book.Id
                };
                book.BookTags.Add(bookTag);
            }

            if (book.MainPhoto == null)
            {
                ModelState.AddModelError("MainPhoto", "Main photo is required");
                return View();
            }
            if (book.HoverPhoto == null)
            {
                ModelState.AddModelError("HoverPhoto", "Hover photo is required");
                return View();
            }
            book.MainImageUrl = book.MainPhoto.SaveFile("image/products");
            book.HoverImageUrl = book.HoverPhoto.SaveFile("image/products");

            if (book.Photos != null)
            {
                book.BookImages = new List<BookImage>();
                foreach (var photo in book.Photos)
                {

                    if (!photo.CheckContentType("image/*"))
                    {
                        ModelState.AddModelError("Photos", "File must be image");
                        return View();
                    }
                    if (photo.CheckFileSize(2))
                    {
                        ModelState.AddModelError("Photos", "Image size must be max 2MB");
                        return View();
                    }
                    var bookImage = new BookImage
                    {
                        ImageUrl = photo.SaveFile("image/products")
                    };
                    book.BookImages.Add(bookImage);
                }
            }
            pustokDbContex.books.Add(book);
            pustokDbContex.SaveChanges();

            return Ok();
        }

        public IActionResult Detail(int id)
        {
            var book = pustokDbContex.books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookImages)
                .FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();
            return View(book);
        }
        public IActionResult BookModal(int? id)
        {
            if(id is null) 
                return NotFound();
            var book = pustokDbContex.books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookImages)
                .Include(b => b.BookTags)
                .ThenInclude(bt => bt.Tag)
                .FirstOrDefault(b => b.Id == id);
            if(book == null) return NotFound();
            return PartialView("_BookModalPartial", book);
        }
    }
}
