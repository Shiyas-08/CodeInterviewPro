using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;
using System.Text;
using System.Text.Json;

namespace CodeInterviewPro.Infrastructure.AI
{
    public class CodeBertService : ICodeBertService
    {
        private readonly HttpClient _httpClient;

        public CodeBertService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CodeBertResult> AnalyzeAsync(string code, string language)
        {
            var request = new
            {
                code = code,
                language = language
            };

            var json = JsonSerializer.Serialize(request);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                "http://localhost:8001/analyze",
                content);

            response.EnsureSuccessStatusCode();

            var responseJson =
                await response.Content.ReadAsStringAsync();

            var result =
                JsonSerializer.Deserialize<CodeBertResult>(
                    responseJson,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return result!;
        }
    }
}