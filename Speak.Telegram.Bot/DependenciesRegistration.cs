﻿using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Speak.Telegram.Audio;
using Speak.Telegram.ChatMigrationFeature;
using Speak.Telegram.CutieFeature;
using Speak.Telegram.MusicQuizFeature;
using Speak.Telegram.PepeFeature;
using Speak.Telegram.Postgres;
using Telegram.Bot;

[assembly: InternalsVisibleTo("Speak.Telegram.Bot.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Speak.Telegram.Bot;

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

        services.AddHostedService<ConfigureWebhookBackgroundService>();
        services.AddScoped<ITelegramMessageRouter, TelegramMessageRouter>();
        services.AddScoped<IMessageHandlerFactory, MessageHandlerFactory>();

        services
            .AddPepeFeature()
            .AddCutieFeature()
            .AddChatMigrationFeature()
            .AddMusicQuizFeature()
            .AddAudioService();

        services.AddPostgresDatabase(configuration);

        return services;
    }

    public static IEndpointRouteBuilder MapBotRoute(this IEndpointRouteBuilder routes, IConfiguration configuration)
    {
        var token = configuration.GetSection(BotConfigurationSectionName).Get<BotConfiguration>()!.Token;
        
        routes.MapControllerRoute(
            HttpClientName,
            $"bot/{token}",
            new { controller = "TelegramWebhook", action = "Post" });
        
        routes.MapControllerRoute(
            HttpClientName,
            $"bot/message/{token}",
            new { controller = "TelegramWebhook", action = "PostMessage" });

        return routes;
    }
}