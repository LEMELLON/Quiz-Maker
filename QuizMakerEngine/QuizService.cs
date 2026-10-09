using System.Text.Json;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using QuizMakerEngine.Data;
using QuizMakerEngine.Data.Entities;
using QuizMakerEngine.Dtos;
using QuizMakerEngine.Models;

namespace QuizMakerEngine.Services;

public interface IQuizService
{
    Task<List<QuizSummaryDto>> GetAllAsync();
    Task<QuizDetailDto?> GetByIdAsync(int id);
    Task<QuizDetailDto> CreateAsync(CreateQuizDto dto);
    Task<QuizDetailDto> SaveGeneratedAsync(string title, string? sourceFileName, List<QuestionCollection> collections);
    Task<QuizSummaryDto?> UpdateTitleAsync(int id, string title);
    Task<bool> DeleteAsync(int id);
}

public class QuizService : IQuizService
{
    private readonly QuizDbContext _db;
    private readonly IMapper _mapper;

    public QuizService(QuizDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public Task<List<QuizSummaryDto>> GetAllAsync() =>
        _db.Quizzes.AsNoTracking()
            .OrderByDescending(q => q.CreatedAt)
            .ProjectTo<QuizSummaryDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

    public async Task<QuizDetailDto?> GetByIdAsync(int id)
    {
        var quiz = await _db.Quizzes.AsNoTracking()
            .Include(q => q.Questions).ThenInclude(q => q.Choices)
            .AsSplitQuery()
            .FirstOrDefaultAsync(q => q.Id == id);
        if (quiz == null) return null;

        quiz.Questions = quiz.Questions.OrderBy(q => q.Position).ToList();
        foreach (var q in quiz.Questions)
            q.Choices = q.Choices.OrderBy(c => c.Position).ToList();

        return _mapper.Map<QuizDetailDto>(quiz);
    }

    public Task<QuizDetailDto> CreateAsync(CreateQuizDto dto) =>
        CreateInternalAsync(dto, JsonSerializer.Serialize(dto));

    public Task<QuizDetailDto> SaveGeneratedAsync(string title, string? sourceFileName, List<QuestionCollection> collections)
    {
        var dto = new CreateQuizDto
        {
            Title = title,
            SourceFileName = sourceFileName,
            Questions = collections.SelectMany(c => c.Variants.Select(v => new CreateQuestionDto
            {
                SourcePage = c.SourcePage,
                TargetTerm = c.TargetTerm,
                QuestionSnippet = c.QuestionSnippet,
                AnswerSnippet = c.AnswerSnippet,
                Tier = c.Tier,
                DefinitionType = c.DefinitionType,
                VariantType = v.Type,
                QuestionText = v.QuestionText,
                Choices = v.Choices.Select((text, i) => new CreateChoiceDto
                {
                    Text = text,
                    IsCorrect = i == v.CorrectAnswerIndex
                }).ToList()
            })).ToList()
        };

        // RawJson keeps the engine's original JSON text of the extraction.
        return CreateInternalAsync(dto, JsonSerializer.Serialize(collections));
    }

    public async Task<QuizSummaryDto?> UpdateTitleAsync(int id, string title)
    {
        title = title.Trim();
        if (title.Length == 0 || title.Length > 200)
            throw new ArgumentException("Title must be 1-200 characters.");

        var quiz = await _db.Quizzes.FindAsync(id);
        if (quiz == null) return null;

        quiz.Title = title;
        await _db.SaveChangesAsync();

        return await _db.Quizzes.AsNoTracking()
            .Where(q => q.Id == id)
            .ProjectTo<QuizSummaryDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var quiz = await _db.Quizzes.FindAsync(id);
        if (quiz == null) return false;

        _db.Quizzes.Remove(quiz);          // cascade removes Questions and Choices
        await _db.SaveChangesAsync();
        return true;
    }

    // ---------- helpers ----------
    private async Task<QuizDetailDto> CreateInternalAsync(CreateQuizDto dto, string rawJson)
    {
        Validate(dto);

        var quiz = _mapper.Map<Quiz>(dto);
        quiz.Title = dto.Title.Trim();
        quiz.RawJson = rawJson;
        quiz.CreatedAt = DateTime.UtcNow;

        for (int i = 0; i < quiz.Questions.Count; i++)
        {
            quiz.Questions[i].Position = i;
            for (int j = 0; j < quiz.Questions[i].Choices.Count; j++)
                quiz.Questions[i].Choices[j].Position = j;
        }

        _db.Quizzes.Add(quiz);
        await _db.SaveChangesAsync();
        return (await GetByIdAsync(quiz.Id))!;
    }

    private static void Validate(CreateQuizDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title) || dto.Title.Trim().Length > 200)
            throw new ArgumentException("Title must be 1-200 characters.");
        if (dto.Questions.Count == 0)
            throw new ArgumentException("A quiz needs at least one question (nothing was generated).");

        foreach (var q in dto.Questions)
        {
            if (q.Choices.Count < 2)
                throw new ArgumentException($"Question '{q.TargetTerm}' needs at least 2 choices.");
            if (q.Choices.Count(c => c.IsCorrect) != 1)
                throw new ArgumentException($"Question '{q.TargetTerm}' must have exactly one correct choice.");
        }
    }
}
