using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenterProject.Data;
using FitnessCenterProject.Models;
using FitnessCenterProject.Models.ViewModels;
using System.Diagnostics;

namespace FitnessCenterProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                Services = await _context.Services.ToListAsync(),
                Trainers = await _context.Trainers.ToListAsync()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}