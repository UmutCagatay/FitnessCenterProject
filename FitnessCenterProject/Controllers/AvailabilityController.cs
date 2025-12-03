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

            var list = await _context.TrainerAvailabilities
                                     .Where(x => x.TrainerId == trainerId)
                                     .ToListAsync();

            ViewBag.Trainer = trainer;
            ViewBag.LastDay = TempData["LastDay"];
            ViewBag.LastStart = TempData["LastStart"];
            ViewBag.LastEnd = TempData["LastEnd"];

            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainerAvailability availability)
        {
            if (availability.EndTime <= availability.StartTime)
            {
                TempData["Error"] = "Bitiş saati, başlangıç saatinden sonra olmalıdır.";
            }
            else if (ModelState.IsValid)
            {
                _context.TrainerAvailabilities.Add(availability);
                await _context.SaveChangesAsync();

                TempData["LastDay"] = availability.DayOfWeek;
                TempData["LastStart"] = availability.StartTime.ToString(@"hh\:mm");
                TempData["LastEnd"] = availability.EndTime.ToString(@"hh\:mm");
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