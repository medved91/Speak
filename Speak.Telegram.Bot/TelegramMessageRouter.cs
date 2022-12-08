using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Speak.Telegram.CommonContracts;
using Speak.Telegram.CutieFeature.Contracts.Requests;
using Speak.Telegram.PepeFeature;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Speak.Telegram.Bot;

internal class TelegramMessageRouter : ITelegramMessageRouter
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<TelegramMessageRouter> _logger;

    private readonly ITelegramFeatureHandler<PickWhichPepeAmITodayFeatureRequest, Message> _pepePickerFeatureHandler;
    private readonly ITelegramFeatureHandler<RegisterInCutieFeatureRequest, Message> _regInCutieFeatureHandler;
    private readonly ITelegramFeatureHandler<StartCutieElectionsFeatureRequest, Message> _startCutieElectionsFeatureHandler;

    public TelegramMessageRouter(ITelegramBotClient botClient,
        ITelegramFeatureHandler<PickWhichPepeAmITodayFeatureRequest, Message> pepePickerFeatureHandler,
        ITelegramFeatureHandler<StartCutieElectionsFeatureRequest, Message> startCutieElectionsFeatureHandler,
        ITelegramFeatureHandler<RegisterInCutieFeatureRequest, Message> regInCutieFeatureHandler,
        ILogger<TelegramMessageRouter> logger)
    {
        _botClient = botClient;
        _logger = logger;
        _startCutieElectionsFeatureHandler = startCutieElectionsFeatureHandler;
        _regInCutieFeatureHandler = regInCutieFeatureHandler;
        _pepePickerFeatureHandler = pepePickerFeatureHandler;
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
        if (message.Type != MessageType.Text) return;
        
        var action = message.Text!.Split(' ')[0] switch
        {
            var pepe when Regex.IsMatch(pepe, @"^\/pepe[@]?") 
                => _pepePickerFeatureHandler.Handle(new PickWhichPepeAmITodayFeatureRequest(
                    message.From?.Username, message.Chat.Id, message.MessageId), ct),
            
            var registerInCutie when Regex.IsMatch(registerInCutie, @"^\/join_cutie[@]?")
                => _regInCutieFeatureHandler.Handle(new RegisterInCutieFeatureRequest(message.From?.Username, 
                    message.From?.FirstName, message.From?.LastName, message.Chat.Id, message.MessageId), ct),
            
            var cutieElections when Regex.IsMatch(cutieElections, @"^\/get_cutie[@]?")
                => _startCutieElectionsFeatureHandler.Handle(new StartCutieElectionsFeatureRequest(message.Chat.Id), ct),
            
            _ => Usage(message)
        };
        
        var sentMessage = await action;
        _logger.LogInformation("Отправлено сообщение с id: {SentMessageId}", sentMessage.MessageId);
    }

    private async Task<Message> Usage(Message message)
    {
        const string usage = "Команды:\n" +
                             "/pepe - узнать, какой ты Pepe сегодня\n";

        return await _botClient.SendTextMessageAsync(message.Chat.Id, text: usage, replyMarkup: new ReplyKeyboardRemove());
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