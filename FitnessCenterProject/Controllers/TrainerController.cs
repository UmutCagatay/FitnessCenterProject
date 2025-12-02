using Microsoft.AspNetCore.Mvc;
using FitnessCenterProject.Data;
using FitnessCenterProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenterProject.Controllers
{
    public class TrainerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Trainers.ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trainer trainer, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string extension = Path.GetExtension(file.FileName);

                    string uniqueFileName = Guid.NewGuid().ToString() + extension;

                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/trainers", uniqueFileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    trainer.ImageUrl = uniqueFileName;
                }

                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null) return NotFound();

            return View(trainer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Trainer trainer, IFormFile? file)
        {
            if (id != trainer.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (file != null)
                    {
                        var oldTrainer = await _context.Trainers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

                        if (oldTrainer?.ImageUrl != null)
                        {
                            string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/trainers", oldTrainer.ImageUrl);
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        string extension = Path.GetExtension(file.FileName);
                        string uniqueFileName = Guid.NewGuid().ToString() + extension;
                        string newPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/trainers", uniqueFileName);

                        using (var stream = new FileStream(newPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        trainer.ImageUrl = uniqueFileName;
                    }
                    else
                    {
                        var oldTrainer = await _context.Trainers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                        trainer.ImageUrl = oldTrainer?.ImageUrl;
                    }

                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Trainers.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null) return NotFound();
            return View(trainer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                // Önce resmi klasörden sil
                if (trainer.ImageUrl != null)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/trainers", trainer.ImageUrl);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                // Sonra kaydı veritabanından sil
                _context.Trainers.Remove(trainer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}