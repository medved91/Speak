using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Speak.Telegram.CommonContracts;
using Speak.Telegram.MusicQuizFeatureContracts;
using Telegram.Bot.Types;

[assembly: InternalsVisibleTo("Speak.Telegram.Bot.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Speak.Telegram.MusicQuizFeature;

public static class DependenciesRegistration
{
    public static IServiceCollection AddMusicQuizFeature(this IServiceCollection services)
    {
        services.AddScoped<ITelegramFeatureHandler<StartMusicQuizFeatureRequest, Message>, StartMusicQuizFeatureHandler>();
        services.AddScoped<ITelegramFeatureHandler<SendMusicQuizAnswerFeatureRequest, Message>, SendMusicQuizAnswerFeatureHandler>();
        
        return services;
    }
}