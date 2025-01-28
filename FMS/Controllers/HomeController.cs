using Microsoft.AspNetCore.Mvc;

namespace FMS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult LoginPage()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RegisterPage()
        {
            return View();
        }
    }
}
