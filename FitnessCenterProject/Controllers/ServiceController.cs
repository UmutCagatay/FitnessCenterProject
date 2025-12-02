using Microsoft.AspNetCore.Mvc;
using FitnessCenterProject.Data;
using FitnessCenterProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenterProject.Controllers
{
    public class ServiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ServiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _context.Services.ToListAsync();
            return View(services);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {
            if (ModelState.IsValid)
            {
                _context.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var errorMessages = ModelState.Values
                                          .SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();

            var fullErrorText = string.Join(" | ", errorMessages);

            throw new Exception("Validasyon Hatası Yakalandı: " + fullErrorText);
        }
    }
}
