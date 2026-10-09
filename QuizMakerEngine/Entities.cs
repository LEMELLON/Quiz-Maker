namespace QuizMakerEngine.Data.Entities;

public class Quiz
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? SourceFileName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Full JSON text of the generated quiz (kept for reference/export).</summary>
    public string RawJson { get; set; } = "{}";

    public List<Question> Questions { get; set; } = new();
}

public class Question
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public int Position { get; set; }
    public int SourcePage { get; set; }
    public string TargetTerm { get; set; } = string.Empty;
    public string QuestionSnippet { get; set; } = string.Empty;
    public string AnswerSnippet { get; set; } = string.Empty;
    public int Tier { get; set; } = 1;
    public string DefinitionType { get; set; } = "new";
    public string VariantType { get; set; } = "clamp";
    public string QuestionText { get; set; } = string.Empty;

    public List<Choice> Choices { get; set; } = new();
}

public class Choice
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public int Position { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
