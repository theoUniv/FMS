using FMS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FMS.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GitHubController : Controller
    {
        private readonly GitHubService _gitHubService;
        private readonly AppDbContext _DBcontext;

        public GitHubController(AppDbContext context, GitHubService gitHubService)
        {
            _gitHubService = gitHubService;
            _DBcontext = context;
        }

        /// <summary>
        /// Action pour récupérer les statistiques des langages de programmation sur GitHub.
        /// </summary>
        /// <returns>Retourne les statistiques des langages sous forme de JSON</returns>
        public async Task<IActionResult> GetLanguageStatsFromGitHub()
        {
            try
            {
                var languages = _DBcontext.GitHubLanguagesData.Select(x => x.nom_langage).ToList();
                var languageStats = await _gitHubService.GetLanguageStatistics(languages);
                
                return Json(languageStats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
        
        public async Task<IActionResult> GetLanguageStats()
        {
            try
            {
                var languages = _DBcontext.GitHubLanguagesData.ToList();
                
                return Json(languages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        public async Task<IActionResult> UpdateAllLangageData()
        {
            try
            {
                var GitHublangages = _DBcontext.GitHubLanguagesData;

                foreach (var langage in GitHublangages)
                {
                    // Mettre à jour le nombre de dépôts pour chaque langage
                    langage.nombre_repertoire = await _gitHubService.GetLanguageRepoNumber(langage.nom_langage);

                    // Assurez-vous que toutes les autres données sont également mises à jour ici si nécessaire

                    _DBcontext.GitHubLanguagesData.Update(langage);
                }

                await _DBcontext.SaveChangesAsync(); // Utiliser SaveChangesAsync pour une gestion optimale des tâches asynchrones
                return Ok(new { Message = "Update effectué !" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        public async Task<IActionResult> UpdateRepoSumByYear()
        {
            try
            {
                var yearsData = _DBcontext.GitHubYearlyStatsModel.ToList();

                foreach (var year in yearsData)
                {
                    year.nombre_repertoire = await _gitHubService.GetRepositoriesCountByYear(year.year);
                    _DBcontext.GitHubYearlyStatsModel.Update(year);
                }

                await _DBcontext.SaveChangesAsync(); // 🔹 Sauvegarde des changements
                return Ok(new { Message = "Update effectué !" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        public async Task<IActionResult> GetRepoSumByYear()
        {
            try
            {
                return Json(_DBcontext.GitHubYearlyStatsModel.ToList());
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
