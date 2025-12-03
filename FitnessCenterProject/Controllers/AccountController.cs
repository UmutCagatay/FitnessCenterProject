using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FitnessCenterProject.Models;
using FitnessCenterProject.Models.ViewModels;

namespace FitnessCenterProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Giriş sayfasını açar
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Giriş yap butonuna basınca
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Şifre ve mail doğru mu bak
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

                if (result.Succeeded)
                {
                    // Başarılıysa ana sayfaya git
                    return RedirectToAction("Index", "Home");
                }

                // Hatalıysa mesaj göster
                ModelState.AddModelError("", "Email veya şifre hatalı.");
            }
            return View(model);
        }

        // Kayıt ol sayfasını açar
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Kayıt ol butonuna basınca
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Yeni kullanıcı oluşturuyoruz
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                // Veritabanına kaydediyoruz
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Kayıt oldu, hemen giriş yap
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                // Hata varsa göster (Şifre kısa vs.)
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        // Çıkış yapma işlemi
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}