using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class DeepSeekService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DeepSeekService> _logger;

    // Constructor injection for HttpClient and ILogger
    public DeepSeekService(HttpClient httpClient, ILogger<DeepSeekService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GetAnswerFromDeepSeek(string userQuery)
    {
        string apiKey = "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";

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
                // Log the successful response
                _logger.LogInformation("Request to OpenRouter API succeeded.");

                // Read the response and extract the content
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return ExtractContentFromResponse(responseString);
            }
            else
            {
                // Log the error and return the error message
                _logger.LogError($"Error: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
                return $"Error: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}";
            }
        }
        catch (HttpRequestException ex)
        {
            // Handle HttpRequestException
            _logger.LogError($"HttpRequestException: {ex.Message}");
            return $"HttpRequestException: {ex.Message}";
        }
        catch (TimeoutException ex)
        {
            // Handle TimeoutException
            _logger.LogError($"TimeoutException: {ex.Message}");
            return $"TimeoutException: {ex.Message}";
        }
        catch (Exception ex)
        {
            // Handle any other exceptions
            _logger.LogError($"Exception: {ex.Message}");
            return $"Exception: {ex.Message}";
        }
    }

    private string ExtractContentFromResponse(string responseString)
    {
        try
        {
            // Deserialize the response and extract the message content
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseString);
            var contentMessage = jsonResponse.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
            return contentMessage ?? "No content available.";
        }
        catch (Exception ex)
        {
            // Log the error if extraction fails
            _logger.LogError($"Error extracting content from response: {ex.Message}");
            return "Error extracting content from response.";
        }
    }
}
