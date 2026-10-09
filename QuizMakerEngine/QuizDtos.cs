using System.ComponentModel.DataAnnotations;

namespace QuizMakerEngine.Dtos;

// ---------- Output ----------
public class ChoiceDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}

public class QuestionDto
{
    public int Id { get; set; }
    public int SourcePage { get; set; }
    public string TargetTerm { get; set; } = string.Empty;
    public int Tier { get; set; }
    public string VariantType { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public List<ChoiceDto> Choices { get; set; } = new();
}

public class QuizSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? SourceFileName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int QuestionCount { get; set; }
}

public class QuizDetailDto : QuizSummaryDto
{
    public List<QuestionDto> Questions { get; set; } = new();
}

// ---------- Input ----------
public class CreateChoiceDto
{
    [Required, StringLength(1000, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}

public class CreateQuestionDto
{
    public int SourcePage { get; set; }
    public string TargetTerm { get; set; } = string.Empty;
    public string QuestionSnippet { get; set; } = string.Empty;
    public string AnswerSnippet { get; set; } = string.Empty;
    public int Tier { get; set; } = 1;
    public string DefinitionType { get; set; } = "new";
    public string VariantType { get; set; } = "clamp";

    [Required]
    public string QuestionText { get; set; } = string.Empty;

    [MinLength(2)]
    public List<CreateChoiceDto> Choices { get; set; } = new();
}

public class CreateQuizDto
{
    [Required, StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    public string? SourceFileName { get; set; }

    [MinLength(1)]
    public List<CreateQuestionDto> Questions { get; set; } = new();
}

public class UpdateQuizDto
{
    [Required, StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;
}
