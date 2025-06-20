using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

public class DeepSeekService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DeepSeekService> _logger;
    private readonly IConfiguration _configuration;

    // Constructor injection for HttpClient, ILogger, and IConfiguration
    public DeepSeekService(HttpClient httpClient, ILogger<DeepSeekService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<string> GetAnswerFromDeepSeek(string userQuery)
    {
        // Get the API key from configuration
        string apiKey = _configuration["OpenRouter:ApiKey"];

        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("API Key is missing.");
            return "API Key is missing.";
        }

        // Set up the request headers
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Prepare the request body
        var requestBody = new
        {
            model = "deepseek/deepseek-r1:free",
            messages = new[] { new { role = "user", content = userQuery } }
        };

        var jsonBody = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        try
        {
            // Send the request to OpenRouter API
            var response = await _httpClient.PostAsync("https://openrouter.ai/api/v1/chat/completions", content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Request to OpenRouter API succeeded.");
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return ExtractContentFromResponse(responseString);
            }
            else
            {
                _logger.LogError($"Error: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
                return $"Error: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}";
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"HttpRequestException: {ex.Message}");
            return $"HttpRequestException: {ex.Message}";
        }
        catch (TimeoutException ex)
        {
            _logger.LogError($"TimeoutException: {ex.Message}");
            return $"TimeoutException: {ex.Message}";
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return $"Exception: {ex.Message}";
        }
    }

    private string ExtractContentFromResponse(string responseString)
    {
        try
        {
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseString);
            var contentMessage = jsonResponse.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
            return contentMessage ?? "No content available.";
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error extracting content from response: {ex.Message}");
            return "Error extracting content from response.";
        }
    }
}
