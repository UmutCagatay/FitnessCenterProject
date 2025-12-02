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

            var trainer = await _context.Trainers
                                        .Include(t => t.TrainerServices)
                                        .FirstOrDefaultAsync(x => x.Id == id);

            if (trainer == null) return NotFound();

            ViewBag.Services = await _context.Services.ToListAsync();

            return View(trainer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // selectedServices: Formdaki checkboxlardan gelen ID listesi
        public async Task<IActionResult> Edit(int id, Trainer trainer, IFormFile? file, int[] selectedServices)
        {
            if (id != trainer.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // --- 1. Veritabanındaki "Gerçek" Eğitmeni ve İlişkilerini Çek ---
                    // AsNoTracking KULLANMIYORUZ çünkü değişiklikleri takip edip kaydetmesini istiyoruz.
                    var existingTrainer = await _context.Trainers
                                                        .Include(t => t.TrainerServices)
                                                        .FirstOrDefaultAsync(t => t.Id == id);

                    if (existingTrainer == null) return NotFound();

                    // --- 2. Temel Bilgileri Güncelle ---
                    existingTrainer.FullName = trainer.FullName;
                    existingTrainer.Specialization = trainer.Specialization;

                    // --- 3. Resim İşlemleri (Aynı Mantık) ---
                    if (file != null)
                    {
                        // Eski resmi sil
                        if (existingTrainer.ImageUrl != null)
                        {
                            string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/trainers", existingTrainer.ImageUrl);
                            if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                        }

                        // Yeni resmi yükle
                        string extension = Path.GetExtension(file.FileName);
                        string uniqueFileName = Guid.NewGuid().ToString() + extension;
                        string newPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/trainers", uniqueFileName);

                        using (var stream = new FileStream(newPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        existingTrainer.ImageUrl = uniqueFileName;
                    }

                    // --- 4. HİZMET EŞLEŞTİRME (EN ÖNEMLİ KISIM) ---
                    // A) Mevcut ilişkilerin hepsini temizle (Reset atıyoruz)
                    if (existingTrainer.TrainerServices != null)
                    {
                        existingTrainer.TrainerServices.Clear();
                    }
                    else
                    {
                        existingTrainer.TrainerServices = new List<TrainerService>();
                    }

                    // B) Seçilen yeni kutucukları ekle
                    foreach (var serviceId in selectedServices)
                    {
                        existingTrainer.TrainerServices.Add(new TrainerService
                        {
                            TrainerId = existingTrainer.Id,
                            ServiceId = serviceId
                        });
                    }

                    // --- 5. Kaydet ---
                    _context.Update(existingTrainer);
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

                if (trainer.ImageUrl != null)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/trainers", trainer.ImageUrl);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }


                _context.Trainers.Remove(trainer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}