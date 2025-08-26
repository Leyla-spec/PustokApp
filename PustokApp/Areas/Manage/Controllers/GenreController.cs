using Microsoft.AspNetCore.Mvc;
using PustokApp.Data;
using PustokApp.Models.BookSlider;

namespace PustokApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class GenreController
        (PustokDbContex pustokDbContex)
        : Controller
    {
        public IActionResult Index()
        {
            var genres = pustokDbContex.Genre.ToList();
            return View(genres);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]

        public IActionResult Create(Genres genre)
        {
            if (!ModelState.IsValid)
                return View();
            if (pustokDbContex.Genre.Any(g => g.Name.ToLower() == genre.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "This genre already exists");
                return View();
            }
            pustokDbContex.Genre.Add(genre);
            pustokDbContex.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int? id)
        {
            var genre = pustokDbContex.Genre.Find(id);
            if (id == null)
                return NotFound();
            if (genre is null)
                return NotFound();
            pustokDbContex.Genre.Remove(genre);
            pustokDbContex.SaveChanges();
            return Ok();

        }
        public IActionResult Edit(int? id)
        {
            var genre = pustokDbContex.Genre.Find(id);
            if (genre is null)
                return NotFound();
            return View(genre);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Genres genre)
        {
            if (!ModelState.IsValid)
                return View();
            var existGenre = pustokDbContex.Genre.Find(genre.Id);
            if (existGenre is null)
                return NotFound();
            if (!ModelState.IsValid)
                return View(existGenre);
            if (pustokDbContex.Genre.Any(g => g.Name.ToLower() == genre.Name.ToLower() && g.Id != genre.Id))
            {
                ModelState.AddModelError("Name", "This genre already exists");
                return View(existGenre);
            }
            existGenre.Name = genre.Name;
            pustokDbContex.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Detail(int? id)
        {
            var genre = pustokDbContex.Genre.Find(id);
            if (id == null)
                return NotFound();
            if (genre is null)
                return NotFound();
            return PartialView("_DetailModal", genre);
        }
    }
}
