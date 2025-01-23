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

        public async Task<IActionResult> Repositories()
        {
            var language = "python";
            var apiResponse = await _gitHubService.GetPopularRepositoriesCount(language);

            return Json(apiResponse);
        }
    }
}
