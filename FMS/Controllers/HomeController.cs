using Microsoft.AspNetCore.Mvc;

namespace FMS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
