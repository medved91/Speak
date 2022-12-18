using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Speak.Telegram.Bot;

internal class TelegramMessageRouter : ITelegramMessageRouter
{
    private readonly ITelegramBotClient _botClient;
    private readonly IMessageHandlerFactory _handlerFactory;
    
    private readonly ILogger<TelegramMessageRouter> _logger;

    public TelegramMessageRouter(ITelegramBotClient botClient, IMessageHandlerFactory handlerFactory, 
        ILogger<TelegramMessageRouter> logger)
    {
        _botClient = botClient;
        _handlerFactory = handlerFactory;
        _logger = logger;
    }

    public async Task HandleNewMessageAsync(Update update, CancellationToken ct)
    {
        var handler = update.Type switch
        {
            UpdateType.Message => BotOnMessageReceivedAsync(update.Message!, ct),
            _ => BotOnUnknownMessageReceivedAsync(update, ct)
        };

        try
        {
            await handler;
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(exception, update.Message?.Chat.Id, ct);
        }
    }

    private async Task BotOnMessageReceivedAsync(Message message, CancellationToken ct)
    {
        _logger.LogInformation("Получено сообщение с типом: {MessageType}", message.Type);

        var handler = _handlerFactory.GetHandlerFor(message, ct);
        
        if (handler == null) return;
        
        var sentMessage = await handler;
        _logger.LogInformation("Отправлено сообщение с id: {SentMessageId}", sentMessage.MessageId);
    }

    private Task BotOnUnknownMessageReceivedAsync(Update update, CancellationToken ct)
    {
        _logger.LogInformation("Неизвестный тип сообщения: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }

    private async Task HandleErrorAsync(Exception exception, long? chatId, CancellationToken ct)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => 
                $"Ошибка Telegram API:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.Message
        };

        _logger.LogError(exception, "Ошибка обработки сообщения Telegram: {ErrorMessage}", errorMessage);

        if (chatId.HasValue)
            await _botClient.SendTextMessageAsync(chatId, 
                "Что-то пошло не так :( Напиши, пожалуйста, @medveden91, либо попробуй позже...", 
                cancellationToken: ct);
    }
}