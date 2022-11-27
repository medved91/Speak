using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Speak.TelegramBot;

public static class DependenciesRegistration
{
    private const string HttpClientName = "telegram";
    private const string BotConfigurationSectionName = "TelegramBot";

    public static IServiceCollection AddBot(this IServiceCollection services, IConfiguration configuration)
    {
        var botConfigurationSection = configuration.GetSection(BotConfigurationSectionName);
        var botConfiguration = botConfigurationSection.Get<BotConfiguration>();
        
        services.Configure<BotConfiguration>(botConfigurationSection);
        
        services.AddHttpClient(HttpClientName)
            .AddTypedClient<ITelegramBotClient>(httpClient => 
                new TelegramBotClient(botConfiguration!.Token, httpClient));

        services.AddScoped<ITelegramMessageRouter, TelegramMessageRouter>();
        services.AddHostedService<ConfigureWebhook>();

        return services;
    }

    public static IEndpointRouteBuilder MapBotRoute(this IEndpointRouteBuilder routes, IConfiguration configuration)
    {
        var token = configuration.GetSection(BotConfigurationSectionName).Get<BotConfiguration>()!.Token;
        
        routes.MapControllerRoute(
            HttpClientName,
            $"bot/{token}",
            new { controller = "TelegramWebhook", action = "Post" });

        return routes;
    }
}