namespace FitnessCenterProject.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentStatus { get; set; }
    }
}
