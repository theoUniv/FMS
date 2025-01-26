using FMS.Services;
using Microsoft.AspNetCore.Mvc;

namespace FMS.Controllers
{
    public class GitHubController : Controller
    {
        private readonly GitHubService _gitHubService;

        public GitHubController(GitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

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
    }
}
