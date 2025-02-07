using FMS.Models;
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

        public async Task<IActionResult> GetUserName()
        {
            var userName = User.Identity.Name;
            return Json(userName);
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
                //var languages = _DBcontext.GitHubLanguagesData.ToList();
                // Ici, pour chaques user, on ne prends plus les langages par défaut
                // On vient se référer à la table d'AssoUserLangage pour obtenir les langages auquels il a droit.
                var userName = User.Identity.Name; // Récupérer le username de l'utilisateur connecté
                int userId = _DBcontext.Users
                    .Where(x => x.username == userName)
                    .Select(x => x.user_id)
                    .FirstOrDefault();

                var languagesIdOfUser = _DBcontext.AssoUserLangage
                    .Where(x => x.id_user == userId)
                    .Select(x => x.id_langage)
                    .ToList();

                var languagesOfUser = _DBcontext.GitHubLanguagesData
                    .Where(x => languagesIdOfUser.Contains((x.id_github_langage_data)))
                    .ToList();

                return Json(languagesOfUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        public async Task<IActionResult> GetLanguages()
        {
            try
            {
                var languages = await _gitHubService.GetAllGitHubLanguages();

                return Json(languages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        public async Task<IActionResult> RetrieveLanguageDataForUser([FromBody] LanguageRequest request)
        {
            try
            {
                var userName = User.Identity.Name; // Récupérer le username de l'utilisateur connecté

                // On vient ajouter la ligne dans la table des langages (le nombre de dépôt sera déjà à jour).
                // S'il n'existe pas déjà.
                if (!_DBcontext.GitHubLanguagesData.Any(x => x.nom_langage == request.Language))
                {
                    _DBcontext.GitHubLanguagesData.Add(new GitHubLangageDataModel
                    {
                        nom_langage = request.Language,
                        nombre_repertoire = await _gitHubService.GetLanguageRepoNumber(request.Language)
                    });
                }

                await _DBcontext
                    .SaveChangesAsync();

                // Puis on créer une ligne d'association dans la table d'ASSO.
                await CreateAssoUserLangage(request.Language, userName);

                return Ok(new { Message = "Update effectué !" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        public async Task<IActionResult> GetUserLangage()
        {
            try
            {
                var userName = User.Identity.Name; // Récupérer le username de l'utilisateur connecté

                var userId = _DBcontext.Users.Where(x => x.username == userName).Select(x => x.user_id)
                    .FirstOrDefault();
                List<int> userIdlangages = _DBcontext.AssoUserLangage
                    .Where(x => x.id_user == userId)
                    .Select(x => x.id_langage).ToList();

                var UserLangages = await _DBcontext.GitHubLanguagesData
                    .Where(x => userIdlangages.Contains(x.id_github_langage_data)).ToListAsync();

                return Json(UserLangages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        public class LanguageRequest
        {
            public string Language { get; set; }
        }

        public async Task<IActionResult> CreateAssoUserLangage(string nom_langages, string username)
        {
            try
            {
                int userID = _DBcontext.Users
                    .Where(x => x.username == username)
                    .Select(x => x.user_id)
                    .FirstOrDefault();

                int langageID = _DBcontext.GitHubLanguagesData
                    .Where(x => x.nom_langage == nom_langages)
                    .Select(x => x.id_github_langage_data)
                    .FirstOrDefault();

                _DBcontext.AssoUserLangage.Add(new AssoUserLangageModel
                {
                    id_langage = langageID,
                    id_user = userID
                });

                await _DBcontext
                    .SaveChangesAsync(); // Utiliser SaveChangesAsync pour une gestion optimale des tâches asynchrones

                return Ok(new { Message = "Update effectué !" });
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
                var GitHublangages = _DBcontext.GitHubLanguagesData.ToList();

                foreach (var langage in GitHublangages)
                {
                    langage.nombre_repertoire = await _gitHubService.GetLanguageRepoNumber(langage.nom_langage);
                    _DBcontext.GitHubLanguagesData.Update(langage);
                }

                await _DBcontext
                    .SaveChangesAsync(); // Utiliser SaveChangesAsync pour une gestion optimale des tâches asynchrones
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

                await _DBcontext.SaveChangesAsync();
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