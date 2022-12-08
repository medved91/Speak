using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Speak.Telegram.CommonContracts;
using Speak.Telegram.PepeFeature.Entities;
using Telegram.Bot.Types;

[assembly: InternalsVisibleTo("Speak.Telegram.Bot.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Speak.Telegram.PepeFeature;

public static class DependenciesRegistration
{
    public static IServiceCollection AddPepeFeature(this IServiceCollection services)
    {
        services.AddSingleton<IRepository<TodayPepe>, TodayPepesRepository>();
        services.AddScoped<ITelegramFeatureHandler<PickWhichPepeAmITodayFeatureRequest, Message>, PickWhichPepeAmITodayHandler>();
        
        return services;
    }
}