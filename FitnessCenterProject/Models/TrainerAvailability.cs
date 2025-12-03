using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models
{
    public class TrainerAvailability
    {
        public int Id { get; set; }

        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        [Required(ErrorMessage = "Gün seçimi zorunludur.")]
        [Display(Name = "Gün")]
        public string DayOfWeek { get; set; }

        [Required(ErrorMessage = "Başlangıç saati zorunludur.")]
        [Display(Name = "Başlangıç Saati")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Bitiş saati zorunludur.")]
        [Display(Name = "Bitiş Saati")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }
    }
}