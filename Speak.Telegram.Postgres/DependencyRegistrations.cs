using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Speak.Telegram.Postgres;

public static class DependencyRegistrations
{
    /// <summary>
    /// Зарегистрировать DbContext
    /// </summary>
    public static IServiceCollection AddPostgresDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TelegramBotDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("Postgres"), builder => 
                builder.MigrationsHistoryTable("__EFMigrationsHistory", TelegramBotDbContext.ServiceSchema)));

        return services;
    }
}