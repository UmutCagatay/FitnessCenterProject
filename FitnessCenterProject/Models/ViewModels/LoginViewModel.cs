using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models.ViewModels
{
    public class LoginViewModel
    {
        // E-posta Alanı
        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        [Display(Name = "Email Adresi")]
        public string Email { get; set; }

        // Şifre Alanı
        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [DataType(DataType.Password)] // Şifrenin yıldızlı (***) görünmesini sağlar
        [Display(Name = "Şifre")]
        public string Password { get; set; }
    }
}