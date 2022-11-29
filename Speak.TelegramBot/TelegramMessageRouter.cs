using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Speak.TelegramBot.FeatureHandlers;
using Speak.TelegramBot.FeatureRequests;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Speak.TelegramBot;

internal class TelegramMessageRouter : ITelegramMessageRouter
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<TelegramMessageRouter> _logger;

    private readonly ITelegramFeatureHandler<PickWhichPepeIAmTodayRequest, Message> _pepePickerFeatureHandler;

    public TelegramMessageRouter(ITelegramBotClient botClient, 
        ITelegramFeatureHandler<PickWhichPepeIAmTodayRequest, Message> pepePickerFeatureHandler, 
        ILogger<TelegramMessageRouter> logger)
    {
        _botClient = botClient;
        _logger = logger;
        _pepePickerFeatureHandler = pepePickerFeatureHandler;
    }

    public async Task HandleNewMessageAsync(Update update)
    {
        var handler = update.Type switch
        {
            UpdateType.Message => BotOnMessageReceivedAsync(update.Message!),
            UpdateType.EditedMessage => BotOnMessageReceivedAsync(update.EditedMessage!),
            _ => BotOnUnknownMessageReceivedAsync(update)
        };

        try
        {
            await handler;
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(exception);
        }
    }

    private async Task BotOnMessageReceivedAsync(Message message)
    {
        _logger.LogInformation("Получено сообщение с типом: {MessageType}", message.Type);
        if (message.Type != MessageType.Text) return;
        
        var action = message.Text!.Split(' ')[0] switch
        {
            var pepe when Regex.IsMatch(pepe, @"^\/pepe[@]?") => 
                _pepePickerFeatureHandler.Handle(new PickWhichPepeIAmTodayRequest(message.Chat.Id)),
            _ => Usage(message)
        };
        
        var sentMessage = await action;
        _logger.LogInformation("Отправлено сообщение с id: {SentMessageId}", sentMessage.MessageId);
    }

    private async Task<Message> Usage(Message message)
    {
        const string usage = "Команды:\n" +
                             "/pepe - получить рандомного Пепе\n";

        return await _botClient.SendTextMessageAsync(message.Chat.Id, text: usage, replyMarkup: new ReplyKeyboardRemove());
    }

    private Task BotOnUnknownMessageReceivedAsync(Update update)
    {
        _logger.LogInformation("Неизвестный тип сообщения: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }

    private Task HandleErrorAsync(Exception exception)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => 
                $"Ошибка Telegram API:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("Ошибка обработки сообщения: {ErrorMessage}", errorMessage);
        return Task.CompletedTask;
    }
}