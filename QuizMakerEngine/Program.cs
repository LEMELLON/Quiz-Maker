using Microsoft.EntityFrameworkCore;
using QuizMakerEngine.Data;
using QuizMakerEngine.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Quiver Quiz Maker API",
        Version = "v1",
        Description = "CRUD API for AI-generated quizzes stored in SQL Server LocalDB."
    });
});

builder.Services.AddQuizMakerEngine(builder.Configuration.GetConnectionString("QuizDb"));

var app = builder.Build();

// Creates the database and applies migrations on startup, so every teammate's machine
// gets the schema automatically (LocalDB is per-machine).
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<QuizDbContext>().Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
