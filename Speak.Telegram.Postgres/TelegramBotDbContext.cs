using Microsoft.EntityFrameworkCore;
using Speak.Telegram.CutieFeature.Contracts;
using Speak.Telegram.Postgres.Configurations;

namespace Speak.Telegram.Postgres;

#nullable disable

public class TelegramBotDbContext : DbContext
{
    internal const string ServiceSchema = "speak";

    public TelegramBotDbContext(DbContextOptions<TelegramBotDbContext> options) : base(options)
    { }

    public DbSet<ChosenCutie> ChosenCuties { get; set; }
    
    public DbSet<CutieMission> CutieMissions { get; set; }
    
    public DbSet<CutiePlayer> CutiePlayers { get; set; }
    
    public DbSet<CutieThinkingPhrase> CutieThinkingPhrases { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfiguration(new CutiePlayerConfiguration())
            .ApplyConfiguration(new CutieMissionConfiguration())
            .ApplyConfiguration(new ChosenCutieConfiguration())
            .ApplyConfiguration(new CutieThinkingPhraseConfiguration());

        modelBuilder.HasDefaultSchema(ServiceSchema);
    }
}