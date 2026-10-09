using Microsoft.EntityFrameworkCore;
using QuizMakerEngine.Data.Entities;

namespace QuizMakerEngine.Data;

public class QuizDbContext : DbContext
{
    public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options) { }

    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Choice> Choices => Set<Choice>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Quiz>(e =>
        {
            e.Property(x => x.Title).IsRequired().HasMaxLength(200);
            e.Property(x => x.SourceFileName).HasMaxLength(260);
            e.Property(x => x.RawJson).HasColumnType("nvarchar(max)");
            e.HasMany(x => x.Questions).WithOne(x => x.Quiz)
             .HasForeignKey(x => x.QuizId).OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Question>(e =>
        {
            e.Property(x => x.TargetTerm).HasMaxLength(300);
            e.Property(x => x.DefinitionType).HasMaxLength(20);
            e.Property(x => x.VariantType).HasMaxLength(30);
            e.HasMany(x => x.Choices).WithOne(x => x.Question)
             .HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
