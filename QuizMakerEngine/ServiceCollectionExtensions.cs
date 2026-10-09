using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuizMakerEngine.Data;
using QuizMakerEngine.Mapping;
using QuizMakerEngine.Services;

namespace QuizMakerEngine.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQuizMakerEngine(this IServiceCollection services, string? connectionString = null)
    {
        services.AddDbContext<QuizDbContext>(o => o.UseSqlServer(connectionString ?? DbDefaults.LocalDb));
        services.AddAutoMapper(cfg => cfg.AddProfile<QuizProfile>());
        services.AddScoped<IQuizService, QuizService>();
        return services;
    }
}
