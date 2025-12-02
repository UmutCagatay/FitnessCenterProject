using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models
{
    public class Trainer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Antrenör adı zorunludur.")]
        [Display(Name = "Ad Soyad")]
        [StringLength(100, ErrorMessage = "İsim en fazla 100 karakter olabilir.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Uzmanlık alanı zorunludur.")]
        [Display(Name = "Uzmanlık Alanı")]
        [StringLength(100)]
        public string Specialization { get; set; }

        [Display(Name = "Fotoğraf")]
        public string? ImageUrl { get; set; }

        public virtual ICollection<TrainerService>? TrainerServices { get; set; }
    }
}