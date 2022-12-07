using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Speak.Telegram.Postgres;

/// <summary>
/// Фабрика DbContext'а для создания миграций.
/// В рантайме не используется, нужна только в момент создания миграций
/// </summary>
internal class SosTrackerDbContextFactory : IDesignTimeDbContextFactory<TelegramBotDbContext>
{
    public TelegramBotDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TelegramBotDbContext>();
        optionsBuilder.UseNpgsql("Host=***;Port=5432;Database=***;Username=***;Password=***;Pooling=true;Maximum Pool Size=10");

        return new TelegramBotDbContext(optionsBuilder.Options);
    }
}