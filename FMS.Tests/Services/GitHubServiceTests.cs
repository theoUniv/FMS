using FMS.Services;
using Moq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq.Protected;

namespace FMS.Tests
{
    public class GitHubServiceTests
    {


        [Fact]
        public async Task GetLanguageStatistics_ShouldReturnValidData_WhenResponseIsSuccessful()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"total_count\": 100, \"items\": [{\"language\": \"C#\"}, {\"language\": \"Python\"}]}")
            };

            // Configure le mock pour qu'il retourne la réponse simulée
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse);

            var httpClient = new HttpClient(mockHandler.Object);
            var gitHubService = new GitHubService(httpClient);

            // Act
            var result = await gitHubService.GetLanguageStatistics(new List<string> { "C#", "Python" });

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("C#"); // Vérifie si le langage C# est présent
            result.Should().ContainKey("Python"); // Vérifie si le langage Python est présent
        }
    }
}
