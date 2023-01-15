using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Speak.Telegram.AudioContracts;

[assembly: InternalsVisibleTo("Speak.Telegram.Bot.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Speak.Telegram.Audio;

public static class DependenciesRegistration
{
    public static IServiceCollection AddAudioService(this IServiceCollection services)
    {
        services.AddSingleton<IAudioService, AudioService>();
        
        return services;
    }
}