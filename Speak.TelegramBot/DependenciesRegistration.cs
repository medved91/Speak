﻿using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Speak.Storage;
using Speak.TelegramBot.Entities;
using Speak.TelegramBot.FeatureHandlers;
using Speak.TelegramBot.FeatureRequests;
using Speak.TelegramBot.Storage;
using Telegram.Bot;
using Telegram.Bot.Types;

[assembly: InternalsVisibleTo("Speak.TelegramBot.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

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

        services.AddHostedService<ConfigureWebhookBackgroundService>();
        services.AddScoped<ITelegramMessageRouter, TelegramMessageRouter>();
        services.AddSingleton<IRepository<TodayPepe>, TodayPepesRepository>();

        services.AddTeleramFeatures();

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

    private static void AddTeleramFeatures(this IServiceCollection services)
    {
        services
            .AddScoped<ITelegramFeatureHandler<PickWhichPepeAmITodayRequest, Message>, PickWhichPepeAmITodayHandler>();
    }
}