using System.Text.Json;
using QuizMakerEngine.Models;
using QuizMakerEngine.Services;

Console.WriteLine("=== Quiz Maker Engine (C# Core) ===");

// 1. Inputs
string apiKey = "AQ.Ab8RN6KJ5eEO8XQ5VdpwjLK44IXqULln4jjBe36xaPwDtM74AQ"; // Set your key here
string documentPath = @"D:\PLV\BSIT 3-8\1st Sem\App Dev\quiz-sample.pdf"; // or .pptx
string model = "gemini-3.1-flash-lite";

if (!File.Exists(documentPath))
{
    Console.WriteLine("File not found. Please provide a valid PDF or PPTX path.");
    return;
}

// 2. Document Extraction
Console.WriteLine("Extracting text from document...");
List<DocumentPage> pages = Path.GetExtension(documentPath).ToLower() == ".pdf"
    ? DocumentExtractor.ExtractPdf(documentPath)
    : DocumentExtractor.ExtractPptx(documentPath);

Console.WriteLine($"Extracted {pages.Count} pages/slides.");

// 3. 3-Tier Multi-Pass Generation
var aiService = new AiService(apiKey);
var finalQuestions = new List<QuestionCollection>();
var accumulatedTerms = new List<string>();

for (int tier = 1; tier <= 2; tier++) // Scan Tiers 1 and 2
{
    Console.WriteLine($"Running Pass: Tier {tier}...");
    string systemPrompt = PromptBuilder.BuildSystemPrompt(tier, accumulatedTerms);
    string userPrompt = PromptBuilder.BuildUserPagesPrompt(pages);

    string rawResponse = await aiService.CallGeminiAsync(model, systemPrompt, userPrompt);
    string cleanJson = AiService.SanitizeJson(rawResponse);

    var response = JsonSerializer.Deserialize<AiTierResponse>(cleanJson);
    if (response?.Collections != null)
    {
        foreach (var raw in response.Collections)
        {
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

            // Build clamp prompt
            if (raw.Variants?.Clamp != null)
            {
                q.Variants.Add(new QuestionVariant
                {
                    Type = "clamp",
                    QuestionText = TextHelpers.ToClampPrompt(raw.Variants.Clamp.Sentence, raw.TargetTerm),
                    Choices = raw.Variants.Clamp.Choices,
                    CorrectAnswerIndex = raw.Variants.Clamp.CorrectAnswerIndex
                });
            }

            finalQuestions.Add(q);
            accumulatedTerms.Add(raw.TargetTerm);
        }
    }
}

Console.WriteLine($"Generated {finalQuestions.Count} question collections.");

// 4. Export to Encrypted Quiz Code
string secretKey = "MySecretPassphrase123";
string quizCode = QuizExporter.ExportToQuizCode(finalQuestions, "Generated Quiz", secretKey);

Console.WriteLine("\n=== EXPORT COMPLETE ===");
Console.WriteLine($"Quiz Code (Base64URL): \n{quizCode}");

// --- Add this to the bottom of Program.cs ---

Console.WriteLine("\n=== DECODING QUIZ ===");

// Pass the newly generated code directly into the decoder
string rawJson = QuizImporter.DecodeQuizCode(quizCode);

// Format the JSON so it is readable in the console
using var parsedDocument = JsonDocument.Parse(rawJson);
string formattedJson = JsonSerializer.Serialize(parsedDocument, new JsonSerializerOptions { WriteIndented = true });

Console.WriteLine(formattedJson);