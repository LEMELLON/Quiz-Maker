using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace QuizMakerEngine.Services;

public class AiService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public AiService(string apiKey)
    {
        _apiKey = apiKey;
        _http = new HttpClient();
    }

    public async Task<string> CallGeminiAsync(string model, string systemPrompt, string userPrompt)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={_apiKey}";

        var payload = new
        {
            systemInstruction = new { parts = new[] { new { text = systemPrompt } } },
            contents = new[] { new { role = "user", parts = new[] { new { text = userPrompt } } } },
            generationConfig = new { responseMimeType = "application/json", temperature = 0.3 }
        };

        var response = await _http.PostAsync(url, new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("candidates")[0]
                              .GetProperty("content")
                              .GetProperty("parts")[0]
                              .GetProperty("text").GetString() ?? string.Empty;
    }

    public static string SanitizeJson(string rawText)
    {
        string text = rawText.Trim();
        if (text.StartsWith("```json", StringComparison.OrdinalIgnoreCase)) text = text[7..];
        else if (text.StartsWith("```")) text = text[3..];
        if (text.EndsWith("```")) text = text[..^3];
        return text.Trim();
    }
}