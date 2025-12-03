using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenterProject.Data;
using FitnessCenterProject.Models;

namespace FitnessCenterProject.Controllers
{
    [Authorize(Roles = "Admin")] // Sadece Admin
    public class TrainerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hocaları listele (Dersleriyle beraber)
        public async Task<IActionResult> Index()
        {
            var trainers = await _context.Trainers
                                 .Include(t => t.TrainerServices)
                                 .ThenInclude(ts => ts.Service)
                                 .ToListAsync();
            return View(trainers);
        }

        // Ekleme sayfası
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Kaydetme işlemi (Resimli)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trainer trainer, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                // Resim varsa kaydet
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
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        // Düzenleme sayfası
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

        // Güncelleme işlemi (Resim + Dersler)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Trainer trainer, IFormFile? file, int[]? selectedServices)
        {
            var mevcutKayit = await _context.Trainers
                                            .Include(t => t.TrainerServices)
                                            .FirstOrDefaultAsync(t => t.Id == id);

            if (mevcutKayit == null) return NotFound();

            // Bilgileri güncelle
            mevcutKayit.FullName = trainer.FullName;
            mevcutKayit.Specialization = trainer.Specialization;

            // Yeni resim varsa değiştir
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

            // Dersleri güncelle
            if (mevcutKayit.TrainerServices != null)
            {
                mevcutKayit.TrainerServices.Clear();
            }
            else
            {
                mevcutKayit.TrainerServices = new List<TrainerService>();
            }

            // Yeni seçilenleri ekle
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
            return RedirectToAction(nameof(Index));
        }

        // Silme sayfası
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // Silme işlemi
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
            return RedirectToAction(nameof(Index));
        }
    }
}