using Newtonsoft.Json.Linq;
using System.Net.Http;

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

        public async Task<int> GetPopularRepositoriesCount(string language)
        {
            string token = "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";
            string apiUrl = $"https://api.github.com/search/repositories?q=language:{language}&sort=stars&order=desc";

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", token);

            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(jsonResponse);

                // Le nombre total de repositories est dans le champ "total_count"
                int totalCount = json["total_count"].Value<int>();

                return totalCount;  // Retourne le nombre total de repositories pour ce langage
            }

            Dispose();

            throw new Exception($"Erreur lors de l'appel à l'API GitHub : {response.StatusCode}");

        }

        // Implémentation de IDisposable pour libérer les ressources HttpClient
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
