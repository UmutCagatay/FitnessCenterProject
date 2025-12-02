using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models
{
    public class AppUser : IdentityUser
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [Display(Name = "Ad")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [Display(Name = "Soyad")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string LastName { get; set; }

        [Display(Name = "Cinsiyet")]
        public string? Gender { get; set; }
    }
}