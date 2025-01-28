using Newtonsoft.Json.Linq;

namespace FMS.Services
{
    public class GitHubService : IDisposable
    {
        private readonly HttpClient _httpClient;

        public GitHubService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ASP.NET MVC App");
        }

        public async Task<Dictionary<string, int>> GetLanguageStatistics()
        {
            string token = "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";
            string[] languages = { "Python", "JavaScript", "Java", "C#", "Go", "Ruby", "PHP", "TypeScript", "C++", "Swift" };

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", token);

            var languageStats = new Dictionary<string, int>();

            foreach (var language in languages)
            {
                string apiUrl = $"https://api.github.com/search/repositories?q=language:{language}&stars:>1";
                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Erreur lors de l'appel à l'API GitHub pour {language}: {response.StatusCode}");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(jsonResponse);

                // Extraire le total des dépôts pour le langage
                int totalCount = json["total_count"]?.Value<int>() ?? 0;
                languageStats[language] = totalCount;
            }

            return languageStats;
        }


        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
