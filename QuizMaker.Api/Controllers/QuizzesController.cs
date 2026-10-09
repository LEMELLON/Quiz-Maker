using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using QuizMakerEngine.Dtos;
using QuizMakerEngine.Services;

namespace QuizMaker.Api.Controllers;

public class GenerateQuizRequest
{
    [Required, StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public IFormFile File { get; set; } = null!;
}

[ApiController]
[Route("api/[controller]")]
public class QuizzesController : ControllerBase
{
    private readonly IQuizService _quizzes;
    private readonly IConfiguration _config;

    public QuizzesController(IQuizService quizzes, IConfiguration config)
    {
        _quizzes = quizzes;
        _config = config;
    }

    /// <summary>List all saved quizzes (summary only).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<QuizSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll() => Ok(await _quizzes.GetAllAsync());

    /// <summary>Get one quiz with all questions and choices.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(QuizDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var quiz = await _quizzes.GetByIdAsync(id);
        return quiz == null ? NotFound(new { error = $"Quiz {id} not found." }) : Ok(quiz);
    }

    /// <summary>Create a quiz manually from JSON (no AI needed).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(QuizDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateQuizDto dto)
    {
        try
        {
            var created = await _quizzes.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>Upload a PDF/PPTX, generate a quiz with Gemini, and save it.</summary>
    [HttpPost("generate")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(30_000_000)]
    [ProducesResponseType(typeof(QuizDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Generate([FromForm] GenerateQuizRequest request)
    {
        string? apiKey = _config["Gemini:ApiKey"] ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
            return BadRequest(new { error = "Gemini API key not configured. Set user-secret 'Gemini:ApiKey' or env var GEMINI_API_KEY." });

        string model = _config["Gemini:Model"] ?? "gemini-3.1-flash-lite";
        string ext = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        string tempPath = Path.Combine(Path.GetTempPath(), $"quiver_{Guid.NewGuid():N}{ext}");

        try
        {
            await using (var fs = System.IO.File.Create(tempPath))
                await request.File.CopyToAsync(fs);

            var collections = await QuizGenerator.GenerateAsync(tempPath, apiKey, model);
            var created = await _quizzes.SaveGeneratedAsync(request.Title, request.File.FileName, collections);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex) when (ex is NotSupportedException or ArgumentException)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { error = $"Gemini request failed: {ex.Message}" });
        }
        finally
        {
            if (System.IO.File.Exists(tempPath)) System.IO.File.Delete(tempPath);
        }
    }

    /// <summary>Rename a quiz.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(QuizSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateQuizDto dto)
    {
        try
        {
            var updated = await _quizzes.UpdateTitleAsync(id, dto.Title);
            return updated == null ? NotFound(new { error = $"Quiz {id} not found." }) : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>Delete a quiz and all its questions/choices.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id) =>
        await _quizzes.DeleteAsync(id) ? NoContent() : NotFound(new { error = $"Quiz {id} not found." });
}
