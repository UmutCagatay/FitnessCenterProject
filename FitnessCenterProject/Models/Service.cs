using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hizmet adı zorunludur.")]
        [Display(Name = "Hizmet Adı")]
        [StringLength(50, ErrorMessage = "Hizmet adı en fazla 50 karakter olabilir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Süre girilmesi zorunludur.")]
        [Range(10, 180, ErrorMessage = "Süre 10 ile 180 dakika arasında olmalıdır.")]
        [Display(Name = "Süre (Dakika)")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Ücret girilmesi zorunludur.")]
        [Range(0, 10000, ErrorMessage = "Ücret 0'dan küçük olamaz.")]
        [Display(Name = "Ücret (₺)")]
        public decimal Price { get; set; }

        [Display(Name = "Hizmet Resmi")]
        public string? ImageUrl { get; set; }

        public virtual ICollection<TrainerService>? TrainerServices { get; set; }
    }
}