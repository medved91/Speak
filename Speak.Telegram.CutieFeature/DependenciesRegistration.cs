using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Speak.Telegram.CommonContracts;
using Speak.Telegram.CutieFeature.Contracts;
using Speak.Telegram.CutieFeature.Contracts.Requests;
using Telegram.Bot.Types;

[assembly: InternalsVisibleTo("Speak.Telegram.Bot.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Speak.Telegram.CutieFeature;

public static class DependenciesRegistration
{
    public static IServiceCollection AddCutieFeature(this IServiceCollection services)
    {
        services.AddScoped<IRepository<CutiePlayer>, CutiePlayersRepository>();
        services.AddScoped<ITelegramFeatureHandler<RegisterInCutieFeatureRequest, Message>, RegisterInCutieFeatureHandler>();
        
        return services;
    }
}