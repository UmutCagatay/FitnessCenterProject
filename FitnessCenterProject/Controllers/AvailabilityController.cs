using Microsoft.AspNetCore.Mvc;
using FitnessCenterProject.Data;
using FitnessCenterProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenterProject.Controllers
{
    public class AvailabilityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AvailabilityController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? trainerId)
        {
            if (trainerId == null) return NotFound();

            var trainer = await _context.Trainers.FindAsync(trainerId);
            if (trainer == null) return NotFound();

            var saatler = await _context.TrainerAvailabilities
                                        .Where(x => x.TrainerId == trainerId)
                                        .ToListAsync();

            ViewBag.Trainer = trainer;

            return View(saatler);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainerAvailability availability)
        {
            if (availability.EndTime <= availability.StartTime)
            {
                TempData["Error"] = "Bitiş saati, başlangıç saatinden büyük olmalıdır.";
                return RedirectToAction("Index", new { trainerId = availability.TrainerId });
            }

            if (ModelState.IsValid)
            {
                _context.TrainerAvailabilities.Add(availability);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { trainerId = availability.TrainerId });
            }

            return RedirectToAction("Index", new { trainerId = availability.TrainerId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var kayit = await _context.TrainerAvailabilities.FindAsync(id);
            if (kayit != null)
            {
                _context.TrainerAvailabilities.Remove(kayit);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", new { trainerId = kayit.TrainerId });
            }
            return NotFound();
        }
    }
}