using FitnessCenterProject.Models;

namespace FitnessCenterProject.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Service> Services { get; set; }
        public List<Trainer> Trainers { get; set; }
    }
}