using System.Text.Json.Serialization;

namespace QuizMakerEngine.Models;

public class DocumentPage
{
    public int PageNumber { get; set; }
    public string Text { get; set; } = string.Empty;
}

public class QuestionCollection
{
    public string Id { get; set; } = $"q_{Guid.NewGuid():N}";
    public int SourcePage { get; set; }
    public string? SourceId { get; set; }
    public string TargetTerm { get; set; } = string.Empty;
    public string SourceSnippet { get; set; } = string.Empty;
    public string QuestionSnippet { get; set; } = string.Empty;
    public string AnswerSnippet { get; set; } = string.Empty;
    public int Tier { get; set; } = 1; // 1 = Essential, 2 = Detail, 3 = Minor
    public string DefinitionType { get; set; } = "new"; // "new" or "extension"
    public List<QuestionVariant> Variants { get; set; } = new();
}

public class QuestionVariant
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "clamp"; // "clamp", "rephrased", "true_false", "category"

    [JsonPropertyName("questionText")]
    public string QuestionText { get; set; } = string.Empty;

    [JsonPropertyName("choices")]
    public List<string> Choices { get; set; } = new();

    [JsonPropertyName("correctAnswerIndex")]
    public int CorrectAnswerIndex { get; set; }

    [JsonPropertyName("correctAnswerIndices")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<int>? CorrectAnswerIndices { get; set; }
}

// Payload format returned by AI prompt calls
public class AiTierResponse
{
    [JsonPropertyName("collections")]
    public List<AiRawCollection> Collections { get; set; } = new();
}

public class AiRawCollection
{
    [JsonPropertyName("targetTerm")]
    public string TargetTerm { get; set; } = string.Empty;

    [JsonPropertyName("sourcePage")]
    public int SourcePage { get; set; }

    [JsonPropertyName("questionSnippet")]
    public string QuestionSnippet { get; set; } = string.Empty;

    [JsonPropertyName("answerSnippet")]
    public string AnswerSnippet { get; set; } = string.Empty;

    [JsonPropertyName("definitionType")]
    public string? DefinitionType { get; set; }

    [JsonPropertyName("variants")]
    public AiRawVariantContainer? Variants { get; set; }
}

public class AiRawVariantContainer
{
    [JsonPropertyName("clamp")]
    public AiRawClamp? Clamp { get; set; }
}

public class AiRawClamp
{
    [JsonPropertyName("sentence")]
    public string Sentence { get; set; } = string.Empty;

    [JsonPropertyName("choices")]
    public List<string> Choices { get; set; } = new();

    [JsonPropertyName("correctAnswerIndex")]
    public int CorrectAnswerIndex { get; set; }
}