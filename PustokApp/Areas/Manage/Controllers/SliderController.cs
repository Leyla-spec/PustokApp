using Microsoft.AspNetCore.Mvc;
using PustokApp.Data;
using PustokApp.Extension;
using PustokApp.Models;


namespace PustokApp.Areas.Manage.Controllers
{
    public class SliderController
        (PustokDbContex pustokDbContext)
        : Controller
    {
        [Area("Manage")]
        public IActionResult Index()
        {
            var sliders = pustokDbContext.Sliders.ToList();
            return View(sliders);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Slider slider)
        {
            if (!ModelState.IsValid)
                return View();
            var file = slider.File;
            if (file == null)
            {
                ModelState.AddModelError("File", "Image is required");
                return View();
            }
            if (!file.CheckContentType("image/*"))
            {
                ModelState.AddModelError("File", "File must be image");
                return View();
            }
            if (file.CheckFileSize(2))
            {
                ModelState.AddModelError("File", "Image size must be max 2MB");
                return View();
            }

            var fileNmae = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/bg-images", fileNmae);
            using (var stream = new FileStream(path, FileMode.Create))
                file.CopyTo(stream);
            slider.ImageUrl = fileNmae;
            slider.CratedAt = DateTime.Now;

            pustokDbContext.Sliders.Add(slider);
            pustokDbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var slider = pustokDbContext.Sliders.Find(id);
            if (slider == null) return NotFound();
            pustokDbContext.Sliders.Remove(slider);
            pustokDbContext.SaveChanges();
            FileManager.DeleteFile("images/bg-images", slider.ImageUrl);
            return Ok();
        }
        public IActionResult Edit(int id)
        {
            var slider = pustokDbContext.Sliders.Find(id);
            if (slider == null) return NotFound();
            return View(slider);
        }
        [HttpPost]
        public IActionResult Edit(Slider slider)
        {
            if (!ModelState.IsValid)
                return View();
            var existSlider = pustokDbContext.Sliders.Find(slider.Id);
            if (existSlider == null) return NotFound();
            var file = slider.File;
            if (file != null)
            {
                if (!file.CheckContentType("image/*"))
                {
                    ModelState.AddModelError("File", "File must be image");
                    return View();
                }
                if (file.CheckFileSize(2))
                {
                    ModelState.AddModelError("File", "Image size must be max 2MB");
                    return View();
                }
                var fileNmae = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/bg-images", fileNmae);
                using (var stream = new FileStream(path, FileMode.Create))
                    file.CopyTo(stream);
                FileManager.DeleteFile("images/bg-images", existSlider.ImageUrl);
                existSlider.ImageUrl = fileNmae;
            }
            existSlider.Title = slider.Title;
            existSlider.Description = slider.Description;
            existSlider.Btntext = slider.Btntext;
            existSlider.BtnLink = slider.BtnLink;
            existSlider.IsActive = slider.IsActive;
            existSlider.Order = slider.Order;
            existSlider.UpdateAt = slider.UpdateAt;
            pustokDbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
