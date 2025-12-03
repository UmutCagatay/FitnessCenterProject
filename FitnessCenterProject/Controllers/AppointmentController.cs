using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenterProject.Data;
using FitnessCenterProject.Models;

namespace FitnessCenterProject.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AppointmentController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (User.IsInRole("Admin"))
            {
                var allAppointments = await _context.Appointments
                                                    .Include(a => a.Trainer)
                                                    .Include(a => a.Service)
                                                    .Include(a => a.AppUser)
                                                    .OrderByDescending(a => a.StartDate)
                                                    .ToListAsync();
                return View(allAppointments);
            }
            else
            {
                var myAppointments = await _context.Appointments
                                                   .Where(a => a.AppUserId == user.Id)
                                                   .Include(a => a.Trainer)
                                                   .Include(a => a.Service)
                                                   .OrderByDescending(a => a.StartDate)
                                                   .ToListAsync();
                return View(myAppointments);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Services = await _context.Services.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment, DateTime tarih, string saat)
        {
            var user = await _userManager.GetUserAsync(User);
            appointment.AppUserId = user.Id;
            appointment.Status = "Pending";
            appointment.CreatedDate = DateTime.Now;

            if (TimeSpan.TryParse(saat, out TimeSpan zaman))
            {
                appointment.StartDate = tarih.Date.Add(zaman);
            }

            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Services = await _context.Services.ToListAsync();
            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);

            if (appointment != null)
            {
                if (!User.IsInRole("Admin") && appointment.AppUserId != user.Id)
                {
                    return Unauthorized();
                }

                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = "Confirmed";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<JsonResult> GetTrainersByService(int serviceId)
        {
            var trainers = await _context.Trainers
                .Where(t => t.TrainerServices.Any(ts => ts.ServiceId == serviceId))
                .Select(t => new { t.Id, t.FullName })
                .ToListAsync();

            return Json(trainers);
        }

        [HttpGet]
        [HttpGet]
        public async Task<JsonResult> GetAvailableSlots(int trainerId, int serviceId, DateTime date)
        {
            var gunler = new Dictionary<DayOfWeek, string>
    {
        { DayOfWeek.Monday, "Pazartesi" }, { DayOfWeek.Tuesday, "Salı" },
        { DayOfWeek.Wednesday, "Çarşamba" }, { DayOfWeek.Thursday, "Perşembe" },
        { DayOfWeek.Friday, "Cuma" }, { DayOfWeek.Saturday, "Cumartesi" },
        { DayOfWeek.Sunday, "Pazar" }
    };
            string gunAdi = gunler[date.DayOfWeek];

            var mesai = await _context.TrainerAvailabilities
                                      .FirstOrDefaultAsync(t => t.TrainerId == trainerId && t.DayOfWeek == gunAdi);

            if (mesai == null) return Json(new List<string>());

            var service = await _context.Services.FindAsync(serviceId);
            if (service == null) return Json(new List<string>());
            int sure = service.Duration;

            var doluRandevular = await _context.Appointments
                                               .Include(a => a.Service)
                                               .Where(a => a.TrainerId == trainerId &&
                                                           a.StartDate.Date == date.Date &&
                                                           a.Status != "Cancelled")
                                               .ToListAsync();

            var slotlar = new List<string>();
            TimeSpan suankiZaman = mesai.StartTime;

            while (suankiZaman.Add(TimeSpan.FromMinutes(sure)) <= mesai.EndTime)
            {
                TimeSpan bitisZamani = suankiZaman.Add(TimeSpan.FromMinutes(sure));

                bool cakisma = doluRandevular.Any(a =>
                    (a.StartDate.TimeOfDay < bitisZamani) &&
                    (a.StartDate.TimeOfDay.Add(TimeSpan.FromMinutes(a.Service.Duration)) > suankiZaman)
                );

                bool gecmis = (date.Date == DateTime.Today && suankiZaman < DateTime.Now.TimeOfDay);

                if (!cakisma && !gecmis)
                {
                    slotlar.Add(suankiZaman.ToString(@"hh\:mm"));
                }

                suankiZaman = suankiZaman.Add(TimeSpan.FromMinutes(sure));
            }

            return Json(slotlar);
        }
    }
}