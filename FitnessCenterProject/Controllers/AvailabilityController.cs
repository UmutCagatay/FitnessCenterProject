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

        // Saatleri listele
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

            // Son seçimleri sayfaya geri gönder
            ViewBag.SeciliGun = TempData["SonGun"];
            ViewBag.SeciliBaslangic = TempData["SonBaslangic"];
            ViewBag.SeciliBitis = TempData["SonBitis"];

            return View(saatler);
        }

        // Yeni saat ekle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainerAvailability availability)
        {
            // Saat mantıksızsa hata ver
            if (availability.EndTime <= availability.StartTime)
            {
                TempData["Error"] = "Bitiş saati, başlangıç saatinden sonra olmalıdır.";
                return RedirectToAction("Index", new { trainerId = availability.TrainerId });
            }

            if (ModelState.IsValid)
            {
                _context.TrainerAvailabilities.Add(availability);
                await _context.SaveChangesAsync();

                // Seçimleri hafızada tut
                TempData["SonGun"] = availability.DayOfWeek;
                TempData["SonBaslangic"] = availability.StartTime.ToString(@"hh\:mm");
                TempData["SonBitis"] = availability.EndTime.ToString(@"hh\:mm");

                return RedirectToAction("Index", new { trainerId = availability.TrainerId });
            }

            return RedirectToAction("Index", new { trainerId = availability.TrainerId });
        }

        // Saati sil
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