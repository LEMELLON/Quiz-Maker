using System.Text.Json;
using QuizMakerEngine.Models;

namespace QuizMakerEngine.Services;

public static class QuizGenerator
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public static async Task<List<QuestionCollection>> GenerateAsync(
        string documentPath,
        string apiKey,
        string model = "gemini-3.1-flash-lite",
        int maxTier = 2,
        Action<string>? log = null)
    {
        if (!File.Exists(documentPath))
            throw new FileNotFoundException("File not found.", documentPath);

        string ext = Path.GetExtension(documentPath).ToLowerInvariant();
        if (ext is not (".pdf" or ".pptx"))
            throw new NotSupportedException("Only .pdf and .pptx files are supported.");

        log?.Invoke("Extracting text from document...");
        List<DocumentPage> pages = ext == ".pdf"
            ? DocumentExtractor.ExtractPdf(documentPath)
            : DocumentExtractor.ExtractPptx(documentPath);
        log?.Invoke($"Extracted {pages.Count} pages/slides.");

        var ai = new AiService(apiKey);
        var results = new List<QuestionCollection>();
        var seenTerms = new List<string>();

        for (int tier = 1; tier <= maxTier; tier++)
        {
            log?.Invoke($"Running pass: Tier {tier}...");
            string systemPrompt = PromptBuilder.BuildSystemPrompt(tier, seenTerms);
            string userPrompt = PromptBuilder.BuildUserPagesPrompt(pages);
            string rawResponse = await ai.CallGeminiAsync(model, systemPrompt, userPrompt);

            AiTierResponse? response;
            try
            {
                response = JsonSerializer.Deserialize<AiTierResponse>(AiService.SanitizeJson(rawResponse), JsonOpts);
            }
            catch (JsonException ex)
            {
                log?.Invoke($"Tier {tier}: AI returned invalid JSON ({ex.Message}). Skipping this pass.");
                continue;
            }

            foreach (var raw in response?.Collections ?? new List<AiRawCollection>())
            {
                var clamp = raw.Variants?.Clamp;
                bool valid = clamp != null
                    && !string.IsNullOrWhiteSpace(raw.TargetTerm)
                    && clamp.Choices.Count >= 2
                    && clamp.CorrectAnswerIndex >= 0
                    && clamp.CorrectAnswerIndex < clamp.Choices.Count;

                if (!valid)
                {
                    log?.Invoke($"  Skipped invalid item: '{raw.TargetTerm}'.");
                    continue;
                }

                var q = new QuestionCollection
                {
                    SourcePage = raw.SourcePage,
                    TargetTerm = raw.TargetTerm,
                    SourceSnippet = raw.QuestionSnippet,
                    QuestionSnippet = raw.QuestionSnippet,
                    AnswerSnippet = raw.AnswerSnippet,
                    Tier = tier,
                    DefinitionType = raw.DefinitionType ?? "new"
                };

                q.Variants.Add(new QuestionVariant
                {
                    Type = "clamp",
                    QuestionText = TextHelpers.ToClampPrompt(clamp!.Sentence, raw.TargetTerm),
                    Choices = clamp.Choices,
                    CorrectAnswerIndex = clamp.CorrectAnswerIndex
                });

                results.Add(q);
                seenTerms.Add(raw.TargetTerm);
            }
        }

        return results;
    }
}
