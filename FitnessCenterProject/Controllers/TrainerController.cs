using Microsoft.AspNetCore.Mvc;

namespace FitnessCenterProject.Controllers
{
    public class Trainer : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
