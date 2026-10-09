using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuizMakerEngine.Data;
using QuizMakerEngine.Dtos;
using QuizMakerEngine.Extensions;
using QuizMakerEngine.Services;

Console.OutputEncoding = Encoding.UTF8;

var provider = new ServiceCollection()
    .AddQuizMakerEngine(Environment.GetEnvironmentVariable("QUIZMAKER_CONNECTION"))
    .BuildServiceProvider();

using (var scope = provider.CreateScope())
    scope.ServiceProvider.GetRequiredService<QuizDbContext>().Database.Migrate();

string? apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
string model = Environment.GetEnvironmentVariable("GEMINI_MODEL") ?? "gemini-3.1-flash-lite";

while (true)
{
    Console.WriteLine();
    Console.WriteLine("==================== QUIVER QUIZ MAKER ====================");
    Console.WriteLine("  1. Create a quiz from a document (PDF / PPTX)");
    Console.WriteLine("  2. List saved quizzes");
    Console.WriteLine("  3. Display a quiz");
    Console.WriteLine("  4. Rename a quiz");
    Console.WriteLine("  5. Delete a quiz");
    Console.WriteLine("  0. Exit");
    Console.WriteLine("===========================================================");
    Console.Write("Choose: ");

    switch (Console.ReadLine()?.Trim())
    {
        case "1": await CreateQuizAsync(); break;
        case "2": await ListQuizzesAsync(); break;
        case "3": await DisplayQuizAsync(); break;
        case "4": await RenameQuizAsync(); break;
        case "5": await DeleteQuizAsync(); break;
        case "0": return;
        default: Console.WriteLine("Invalid choice."); break;
    }
}

// ---------------------------------------------------------------- actions
async Task CreateQuizAsync()
{
    if (string.IsNullOrWhiteSpace(apiKey))
    {
        Console.Write("GEMINI_API_KEY not set. Paste your key (kept for this session only): ");
        apiKey = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(apiKey)) { Console.WriteLine("Cancelled."); return; }
    }

    Console.Write("Path to PDF/PPTX (you can drag the file into this window): ");
    string path = (Console.ReadLine() ?? "").Trim().Trim('"');
    if (path.Length == 0) { Console.WriteLine("Cancelled."); return; }

    Console.Write("Quiz title (Enter = file name): ");
    string title = (Console.ReadLine() ?? "").Trim();
    if (title.Length == 0) title = Path.GetFileNameWithoutExtension(path);

    try
    {
        var collections = await QuizGenerator.GenerateAsync(path, apiKey!, model, 2, msg => Console.WriteLine("  " + msg));

        using var scope = provider.CreateScope();
        var svc = scope.ServiceProvider.GetRequiredService<IQuizService>();
        var saved = await svc.SaveGeneratedAsync(title, Path.GetFileName(path), collections);
        Console.WriteLine($"Saved quiz #{saved.Id} \"{saved.Title}\" with {saved.QuestionCount} questions.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Could not create quiz: {ex.Message}");
    }
}

async Task ListQuizzesAsync()
{
    using var scope = provider.CreateScope();
    var list = await scope.ServiceProvider.GetRequiredService<IQuizService>().GetAllAsync();
    PrintList(list);
}

async Task DisplayQuizAsync()
{
    using var scope = provider.CreateScope();
    var svc = scope.ServiceProvider.GetRequiredService<IQuizService>();
    var list = await svc.GetAllAsync();
    if (list.Count == 0) { Console.WriteLine("No saved quizzes yet."); return; }
    PrintList(list);

    int? id = ReadId("Quiz id to display (Enter = cancel): ");
    if (id == null) return;

    var quiz = await svc.GetByIdAsync(id.Value);
    if (quiz == null) { Console.WriteLine("Quiz not found."); return; }

    Console.Write("Show answer key? (y/N): ");
    bool showKey = (Console.ReadLine() ?? "").Trim().Equals("y", StringComparison.OrdinalIgnoreCase);

    string bar = new string('=', 70);
    Console.WriteLine();
    Console.WriteLine(bar);
    Console.WriteLine($" QUIZ #{quiz.Id}: {quiz.Title}");
    Console.WriteLine($" Source: {quiz.SourceFileName ?? "-"}   Created: {quiz.CreatedAt.ToLocalTime():yyyy-MM-dd HH:mm}   Questions: {quiz.Questions.Count}");
    Console.WriteLine(bar);

    int n = 1;
    foreach (var q in quiz.Questions)
    {
        Console.WriteLine();
        Console.WriteLine($"Q{n++}. (Page {q.SourcePage} | Tier {q.Tier})");
        Console.WriteLine($"     {q.QuestionText}");
        for (int i = 0; i < q.Choices.Count; i++)
        {
            string mark = showKey && q.Choices[i].IsCorrect ? " <-- correct" : "";
            Console.WriteLine($"     {(char)('A' + i)}. {q.Choices[i].Text}{mark}");
        }
    }
    Console.WriteLine();
    Console.WriteLine(bar);
}

async Task RenameQuizAsync()
{
    using var scope = provider.CreateScope();
    var svc = scope.ServiceProvider.GetRequiredService<IQuizService>();
    var list = await svc.GetAllAsync();
    if (list.Count == 0) { Console.WriteLine("No saved quizzes yet."); return; }
    PrintList(list);

    int? id = ReadId("Quiz id to rename (Enter = cancel): ");
    if (id == null) return;

    Console.Write("New title: ");
    string title = Console.ReadLine() ?? "";
    try
    {
        var updated = await svc.UpdateTitleAsync(id.Value, title);
        Console.WriteLine(updated == null ? "Quiz not found." : $"Renamed to \"{updated.Title}\".");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine(ex.Message);
    }
}

async Task DeleteQuizAsync()
{
    using var scope = provider.CreateScope();
    var svc = scope.ServiceProvider.GetRequiredService<IQuizService>();
    var list = await svc.GetAllAsync();
    if (list.Count == 0) { Console.WriteLine("No saved quizzes yet."); return; }
    PrintList(list);

    int? id = ReadId("Quiz id to delete (Enter = cancel): ");
    if (id == null) return;

    Console.Write($"Delete quiz #{id}? This cannot be undone. (y/N): ");
    if (!(Console.ReadLine() ?? "").Trim().Equals("y", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Cancelled.");
        return;
    }
    Console.WriteLine(await svc.DeleteAsync(id.Value) ? "Deleted." : "Quiz not found.");
}

// ---------------------------------------------------------------- helpers
void PrintList(List<QuizSummaryDto> list)
{
    if (list.Count == 0) { Console.WriteLine("No saved quizzes yet."); return; }
    Console.WriteLine();
    Console.WriteLine($"{"ID",-5}{"Title",-38}{"Questions",-11}{"Created",-18}");
    Console.WriteLine(new string('-', 72));
    foreach (var q in list)
    {
        string t = q.Title.Length > 36 ? q.Title[..35] + "…" : q.Title;
        Console.WriteLine($"{q.Id,-5}{t,-38}{q.QuestionCount,-11}{q.CreatedAt.ToLocalTime():yyyy-MM-dd HH:mm}");
    }
}

int? ReadId(string prompt)
{
    Console.Write(prompt);
    string? input = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(input)) return null;
    if (int.TryParse(input, out int id)) return id;
    Console.WriteLine("Not a valid number.");
    return null;
}
