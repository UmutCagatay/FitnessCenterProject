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
    }
}