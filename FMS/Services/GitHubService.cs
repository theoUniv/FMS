using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace FMS.Services
{
    /// <summary>
    /// Service pour interagir avec l'API GitHub et récupérer des statistiques sur les langages de programmation.
    /// </summary>
    public class GitHubService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken = "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";

        /// <summary>
        /// Constructeur de service qui initialise un client HTTP avec un en-tête "User-Agent".
        /// </summary>
        /// <param name="httpClient">Instance de HttpClient injectée pour effectuer les requêtes HTTP.</param>
        public GitHubService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ASP.NET MVC App");
        }

        /// <summary>
        /// Récupère les statistiques des langages de programmation les plus populaires sur GitHub.
        /// </summary>
        /// <returns>Un dictionnaire contenant les noms des langages comme clés et le nombre de dépôts comme valeurs.</returns>
        /// <exception cref="Exception">Lance une exception en cas d'échec de la requête HTTP.</exception>
        public async Task<Dictionary<string, int>> GetLanguageStatistics(List<string> languages)
        {
            //string[] languages = { "Python", "JavaScript", "Java", "C#", "Go", "Ruby", "PHP", "TypeScript", "C++", "Swift" };

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("token", _accessToken);

            var languageStats = new Dictionary<string, int>();

            foreach (var language in languages)
            {
                string apiUrl = $"https://api.github.com/search/repositories?q=language:{language}&stars:>1";
                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(
                        $"Erreur lors de l'appel à l'API GitHub pour {language}: {response.StatusCode}");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(jsonResponse);

                // Extraire le total des dépôts pour le langage
                int totalCount = json["total_count"]?.Value<int>() ?? 0;
                languageStats[language] = totalCount;
            }

            return languageStats;
        }

        public async Task<int> GetLanguageRepoNumber(string langage)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("token", _accessToken);

            var languageStats = new Dictionary<string, int>();

            string apiUrl = $"https://api.github.com/search/repositories?q=language:{langage}&stars:>1";
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erreur lors de l'appel à l'API GitHub pour {langage}: {response.StatusCode}");
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(jsonResponse);

            // Extraire le total des dépôts pour le langage
            int totalCount = json["total_count"]?.Value<int>() ?? 0;
            languageStats[langage] = totalCount;

            return totalCount;
        }

        public async Task<int> GetRepositoriesCountByYear(int year)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("token", _accessToken);

                string apiUrl = $"https://api.github.com/search/repositories?q=created:{year}-01-01..{year}-12-31";

                var response = await _httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Erreur API GitHub pour l'année {year}: {response.StatusCode}");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(jsonResponse);

                return json["total_count"]?.Value<int>() ?? 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }


        /// <summary>
        /// Libère les ressources utilisées par l'instance de HttpClient.
        /// </summary>
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}