using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using E_Commerce_Platform_Ass1.Service.Options;
using E_Commerce_Platform_Ass1.Service.Services.IServices;
using Microsoft.Extensions.Options;

namespace E_Commerce_Platform_Ass1.Service.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiSettings _settings;
        private readonly JsonSerializerOptions _jsonOptions;

        public GeminiService(HttpClient httpClient, IOptions<GeminiSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task<string?> GenerateContentAsync(string prompt)
        {
            try
            {
                var url = $"{_settings.BaseUrl}/models/{_settings.Model}:generateContent?key={_settings.ApiKey}";

                var request = new
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
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        maxOutputTokens = 2048
                    }
                };

                var response = await _httpClient.PostAsJsonAsync(url, request, _jsonOptions);
                
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Gemini API Error: {error}");
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<GeminiResponse>(_jsonOptions);
                
                return result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gemini API Exception: {ex.Message}");
                return null;
            }
        }

        public async Task<T?> GenerateJsonContentAsync<T>(string prompt) where T : class
        {
            var jsonPrompt = prompt + "\n\nIMPORTANT: Return ONLY valid JSON, no markdown, no explanation, no code blocks.";
            
            var response = await GenerateContentAsync(jsonPrompt);
            
            if (string.IsNullOrEmpty(response))
                return null;

            try
            {
                // Clean up response - remove markdown code blocks if present
                var cleanJson = response
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                return JsonSerializer.Deserialize<T>(cleanJson, _jsonOptions);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Parse Error: {ex.Message}");
                Console.WriteLine($"Response was: {response}");
                return null;
            }
        }

        #region Response Models

        private class GeminiResponse
        {
            public List<GeminiCandidate>? Candidates { get; set; }
        }

        private class GeminiCandidate
        {
            public GeminiContent? Content { get; set; }
        }

        private class GeminiContent
        {
            public List<GeminiPart>? Parts { get; set; }
        }

        private class GeminiPart
        {
            public string? Text { get; set; }
        }

        #endregion
    }
}
