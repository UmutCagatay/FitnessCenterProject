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

        // LİSTELEME
        public async Task<IActionResult> Index()
        {
            return View(await _context.Trainers.ToListAsync());
        }

        // EKLEME SAYFASI
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // EKLEME İŞLEMİ (Resim Yüklemeli)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trainer trainer, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    string yol = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/trainers", dosyaAdi);

                    using (var stream = new FileStream(yol, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    trainer.ImageUrl = dosyaAdi;
                }

                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(trainer);
        }

        // DÜZENLEME SAYFASI
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers
                                        .Include(t => t.TrainerServices)
                                        .FirstOrDefaultAsync(x => x.Id == id);

            if (trainer == null) return NotFound();

            ViewBag.Services = await _context.Services.ToListAsync();

            return View(trainer);
        }

        // DÜZENLEME İŞLEMİ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Trainer trainer, IFormFile? file, int[]? selectedServices)
        {
            var mevcutKayit = await _context.Trainers
                                            .Include(t => t.TrainerServices)
                                            .FirstOrDefaultAsync(t => t.Id == id);

            if (mevcutKayit == null) return NotFound();

            mevcutKayit.FullName = trainer.FullName;
            mevcutKayit.Specialization = trainer.Specialization;

            if (file != null)
            {
                string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string yol = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/trainers", dosyaAdi);

                using (var stream = new FileStream(yol, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                mevcutKayit.ImageUrl = dosyaAdi;
            }

            if (mevcutKayit.TrainerServices != null)
            {
                mevcutKayit.TrainerServices.Clear();
            }
            else
            {
                mevcutKayit.TrainerServices = new List<TrainerService>();
            }

            if (selectedServices != null)
            {
                foreach (var servisId in selectedServices)
                {
                    mevcutKayit.TrainerServices.Add(new TrainerService
                    {
                        TrainerId = id,
                        ServiceId = servisId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // SİLME SAYFASI
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null) return NotFound();
            return View(trainer);
        }

        // SİLME İŞLEMİ
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                _context.Trainers.Remove(trainer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}