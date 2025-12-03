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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Services = await _context.Services.ToListAsync();
            ViewBag.Trainers = await _context.Trainers.ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            ViewBag.Services = await _context.Services.ToListAsync();
            ViewBag.Trainers = await _context.Trainers.ToListAsync();

            var user = await _userManager.GetUserAsync(User);
            appointment.AppUserId = user.Id;
            appointment.Status = "Pending";
            appointment.CreatedDate = DateTime.Now;

            if (appointment.StartDate < DateTime.Now)
            {
                ModelState.AddModelError("", "Geçmiş bir tarihe randevu alamazsınız.");
                return View(appointment);
            }

            bool dersVeriyorMu = await _context.TrainerServices
                                               .AnyAsync(x => x.TrainerId == appointment.TrainerId &&
                                                              x.ServiceId == appointment.ServiceId);
            if (!dersVeriyorMu)
            {
                ModelState.AddModelError("", "Seçilen eğitmen bu hizmeti vermemektedir.");
                return View(appointment);
            }

            var gunler = new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Monday, "Pazartesi" },
                { DayOfWeek.Tuesday, "Salı" },
                { DayOfWeek.Wednesday, "Çarşamba" },
                { DayOfWeek.Thursday, "Perşembe" },
                { DayOfWeek.Friday, "Cuma" },
                { DayOfWeek.Saturday, "Cumartesi" },
                { DayOfWeek.Sunday, "Pazar" }
            };
            string secilenGun = gunler[appointment.StartDate.DayOfWeek];

            var calismaSaati = await _context.TrainerAvailabilities
                                             .FirstOrDefaultAsync(x => x.TrainerId == appointment.TrainerId &&
                                                                       x.DayOfWeek == secilenGun);

            if (calismaSaati == null)
            {
                ModelState.AddModelError("", $"Eğitmen {secilenGun} günü çalışmamaktadır.");
                return View(appointment);
            }

            var service = await _context.Services.FindAsync(appointment.ServiceId);
            if (service == null) return NotFound();

            TimeSpan baslangic = appointment.StartDate.TimeOfDay;
            TimeSpan bitis = baslangic.Add(TimeSpan.FromMinutes(service.Duration));

            if (baslangic < calismaSaati.StartTime || bitis > calismaSaati.EndTime)
            {
                ModelState.AddModelError("", $"Eğitmen bu saatlerde müsait değil. (Çalışma Saatleri: {calismaSaati.StartTime.ToString(@"hh\:mm")} - {calismaSaati.EndTime.ToString(@"hh\:mm")})");
                return View(appointment);
            }

            var cakismaVarMi = await _context.Appointments
                .Include(a => a.Service)
                .AnyAsync(a =>
                    a.TrainerId == appointment.TrainerId &&
                    a.StartDate.Date == appointment.StartDate.Date &&
                    a.Status != "Cancelled" &&
                    (
                        a.StartDate.TimeOfDay < bitis &&
                        a.StartDate.TimeOfDay.Add(TimeSpan.FromMinutes(a.Service.Duration)) > baslangic
                    )
                );

            if (cakismaVarMi)
            {
                ModelState.AddModelError("", "Seçilen saatte eğitmenin başka bir randevusu mevcut.");
                return View(appointment);
            }

            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(appointment);
        }
    }
}