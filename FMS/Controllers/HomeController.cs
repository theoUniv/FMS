using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMS.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Action qui retourne la vue de la page d'accueil.
        /// </summary>
        /// <returns>Retourne la vue par d?faut de l'index.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Action pour afficher la page de connexion.
        /// </summary>
        /// <returns>Retourne la vue pour la page de connexion.</returns>
        [HttpGet]
        public IActionResult LoginPage()
        {
            return View();
        }

        /// <summary>
        /// Action pour afficher la page d'inscription.
        /// </summary>
        /// <returns>Retourne la vue pour la page d'inscription.</returns>
        [HttpGet]
        public IActionResult RegisterPage()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Langages()
        {
            return View();
        }
    }
}