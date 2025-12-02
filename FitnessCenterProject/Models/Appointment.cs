using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Üye")]
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        [Display(Name = "Eğitmen")]
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }

        [Display(Name = "Hizmet")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [Required(ErrorMessage = "Tarih seçilmelidir.")]
        [Display(Name = "Randevu Tarihi")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Durum")]
        public string Status { get; set; } = "Pending";

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}