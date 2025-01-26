using FMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMS.Controllers
{
    [Authorize]  // Cette ligne assure que l'utilisateur doit être authentifié
    public class GitHubController : Controller
    {
        private readonly GitHubService _gitHubService;

        public GitHubController(GitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        // Action pour récupérer les statistiques des langages
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

        // Vous pouvez également avoir une action pour retourner la page
        public IActionResult Index()
        {
            return View();  // Rendre la vue qui contient votre graphique
        }
    }
}
