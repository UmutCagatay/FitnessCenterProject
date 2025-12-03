using FitnessCenterProject.Data;
using FitnessCenterProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenterProject.Controllers
{
    [Authorize(Roles = "Admin")]
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
                TempData["Error"] = "Bitiş saati, başlangıç saatinden sonra olmalıdır.";
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
            var item = await _context.TrainerAvailabilities.FindAsync(id);
            if (item != null)
            {
                _context.TrainerAvailabilities.Remove(item);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { trainerId = item.TrainerId });
            }
            return NotFound();
        }
    }
}