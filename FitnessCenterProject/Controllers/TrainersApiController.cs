using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenterProject.Data;
using FitnessCenterProject.Models;

namespace FitnessCenterProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TrainersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // API'den hoca listesi (Filtreleme var)
        [HttpGet]
        public async Task<IActionResult> GetTrainers(string? uzmanlik)
        {
            var query = _context.Trainers.AsQueryable();

            if (!string.IsNullOrEmpty(uzmanlik))
            {
                // Ders adına göre filtrele
                query = query.Where(t => t.TrainerServices.Any(ts => ts.Service.Name.Contains(uzmanlik)));
            }

            var trainers = await query
                .Select(t => new
                {
                    Id = t.Id,
                    AdSoyad = t.FullName,
                    Unvan = t.Specialization,
                    VerdigiDersler = t.TrainerServices.Select(ts => ts.Service.Name).ToList(),
                    Resim = t.ImageUrl != null ? "/images/trainers/" + t.ImageUrl : "Resim Yok"
                })
                .ToListAsync();

            return Ok(trainers);
        }
    }
}