using FMS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMS.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GitHubController : Controller
    {
        private readonly GitHubService _gitHubService;

        public GitHubController(GitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        /// <summary>
        /// Action pour récupérer les statistiques des langages de programmation sur GitHub.
        /// </summary>
        /// <returns>Retourne les statistiques des langages sous forme de JSON</returns>
        public async Task<IActionResult> GetLanguageStats()
        {
            try
            {
                var languageStats = await _gitHubService.GetLanguageStatistics();
                return Json(languageStats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Action pour afficher la page contenant le graphique.
        /// </summary>
        /// <returns>Retourne la vue pour afficher le graphique des langages.</returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
