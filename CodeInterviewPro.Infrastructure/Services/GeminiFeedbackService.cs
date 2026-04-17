using CodeInterviewPro.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Net;

namespace CodeInterviewPro.Infrastructure.Services
{
    public class GeminiFeedbackService : IAIFeedbackService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        public GeminiFeedbackService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new Exception("API Key not found in environment variables");
            }
        }
        //public GeminiFeedbackService(
        //    HttpClient httpClient,
        //    IConfiguration configuration)
        //{
        //    _httpClient = httpClient;
        //    _apiKey = configuration["Gemini:ApiKey"];
        //}

        public async Task<string> GenerateAsync(
            string question,
            string description,
            string code,
            string language,
            string complexity,
            double score)
        {
            var prompt = $@"
You are a senior technical interviewer.

Question:
{question}

Description:
{description}

Language:
{language}

Candidate Code:
{code}

Complexity:
{complexity}

Score:
{score}

Provide:
1. Correctness
2. Performance
3. Code quality
4. Suggestions
5. Hiring recommendation
";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            int retry = 3;

            while (retry > 0)
            {
                // FIX: Updated model name to gemini-3-flash-preview for the 2026 v1beta endpoint
                // This resolves the 404 error while maintaining the 'Flash' speed you need.
                var modelName = "gemini-3-flash-preview";

                var url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelName}:generateContent";

                var request = new HttpRequestMessage(HttpMethod.Post, url);

                request.Headers.Add("x-goog-api-key", _apiKey);

                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);

                //var modelName = "gemini-3-flash-preview";
                //var url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelName}:generateContent?key={_apiKey}";

                //var request = new HttpRequestMessage(HttpMethod.Post, url)
                //{
                //    Content = new StringContent(json, Encoding.UTF8, "application/json")
                //};

                //var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();

                    using var doc = JsonDocument.Parse(responseJson);
                    var result = doc.RootElement
                                   .GetProperty("candidates")[0]
                                   .GetProperty("content")
                                   .GetProperty("parts")[0]
                                   .GetProperty("text")
                                   .GetString();

                    return result!;
                }

                // Handle 429 (Rate Limit) AND 503 (Server Busy/Unavailable)
                if (response.StatusCode == HttpStatusCode.TooManyRequests ||
                    response.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    // Exponential backoff: Wait longer with each failure (3s, 6s, 9s)
                    await Task.Delay(3000 * (4 - retry));
                    retry--;
                    continue;
                }

                // Log the error details if it's not a retryable error
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Gemini API Error: {response.StatusCode} - {error}");

                response.EnsureSuccessStatusCode();
            }

            throw new Exception("Gemini API failed after all retry attempts.");
        }
    }
}
//using CodeInterviewPro.Application.Interfaces.Services;
 //using Microsoft.Extensions.Configuration;
 //using System.Text;
 //using System.Text.Json;

//namespace CodeInterviewPro.Infrastructure.Services
//{
//    public class GeminiFeedbackService : IAIFeedbackService
//    {
//        private readonly HttpClient _httpClient;
//        private readonly string _apiKey;

//        public GeminiFeedbackService(
//            HttpClient httpClient,
//            IConfiguration configuration)
//        {
//            _httpClient = httpClient;
//            _apiKey = configuration["Gemini:ApiKey"];
//        }

//        public async Task<string> GenerateAsync(
//            string question,
//            string description,
//            string code,
//            string language,
//            string complexity,
//            double score)
//        {
//            var prompt = $@"
//You are a senior technical interviewer.

//Question:
//{question}

//Description:
//{description}

//Language:
//{language}

//Candidate Code:
//{code}

//Complexity:
//{complexity}

//Score:
//{score}

//Provide:

//1. Correctness
//2. Performance
//3. Code quality
//4. Suggestions
//5. Hiring recommendation
//";

//            var requestBody = new
//            {
//                contents = new[]
//                {
//                    new
//                    {
//                        parts = new[]
//                        {
//                            new
//                            {
//                                text = prompt
//                            }
//                        }
//                    }
//                }
//            };

//            var json = JsonSerializer.Serialize(requestBody);

//            int retry = 3;

//            while (retry > 0)
//            {
//                var request = new HttpRequestMessage(
//               HttpMethod.Post,
//               $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}");

//                request.Content = new StringContent(
//                    json,
//                    Encoding.UTF8,
//                    "application/json");

//                var response = await _httpClient.SendAsync(request);

//                if (response.IsSuccessStatusCode)
//                {
//                    var responseJson =
//                        await response.Content.ReadAsStringAsync();

//                    using var doc =
//                        JsonDocument.Parse(responseJson);

//                    var result =
//                        doc.RootElement
//                           .GetProperty("candidates")[0]
//                           .GetProperty("content")
//                           .GetProperty("parts")[0]
//                           .GetProperty("text")
//                           .GetString();

//                    return result!;
//                }

//                if ((int)response.StatusCode == 429)
//                {
//                    await Task.Delay(3000);
//                    retry--;
//                    continue;
//                }

//                var error =
//                    await response.Content.ReadAsStringAsync();

//                Console.WriteLine(error);

//                response.EnsureSuccessStatusCode();
//            }

//            throw new Exception("Gemini API failed after retries");
//        }
//    }
//}