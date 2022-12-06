using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Speak.Telegram.Bot;

internal class ConfigureWebhookBackgroundService : IHostedService
{
    private readonly ILogger<ConfigureWebhookBackgroundService> _logger;
    private readonly IServiceProvider _services;
    private readonly BotConfiguration _configuration;

    public ConfigureWebhookBackgroundService(ILogger<ConfigureWebhookBackgroundService> logger,
        IServiceProvider serviceProvider,
        IOptions<BotConfiguration> configuration)
    {
        _logger = logger;
        _services = serviceProvider;
        _configuration = configuration.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        var webhookAddress = @$"{_configuration.Host}/bot/{_configuration.Token}";
        _logger.LogInformation("Устанавливаем Webhook: {WebhookAddress}", webhookAddress);
        await botClient.SetWebhookAsync(
            url: webhookAddress,
            allowedUpdates: Array.Empty<UpdateType>(),
            cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        _logger.LogInformation("Убираем Webhook");
        await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
    }
}
