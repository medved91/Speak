using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Speak.Telegram.ChatMigrationFeatureContracts;
using Speak.Telegram.CommonContracts;
using Telegram.Bot.Types;

[assembly: InternalsVisibleTo("Speak.Telegram.Bot.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Speak.Telegram.ChatMigrationFeature;

public static class DependenciesRegistration
{
    public static IServiceCollection AddChatMigrationFeature(this IServiceCollection services)
    {
        services.AddScoped<ITelegramFeatureHandler<MigrateChatFeatureRequest, Message>, MigrateChatFeatureHandler>();
        
        return services;
    }
}