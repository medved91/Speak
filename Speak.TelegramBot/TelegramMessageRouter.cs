using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace Speak.TelegramBot;

internal class TelegramMessageRouter : ITelegramMessageRouter
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<TelegramMessageRouter> _logger;

    public TelegramMessageRouter(ITelegramBotClient botClient, ILogger<TelegramMessageRouter> logger)
    {
        _botClient = botClient;
        _logger = logger;
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
            "/pepe" => SendPepe(message),
            _ => Usage(message)
        };
        
        var sentMessage = await action;
        _logger.LogInformation("Отправлено сообщение с id: {SentMessageId}", sentMessage.MessageId);
    }
    
    private async Task<Message> SendPepe(Message message)
    {
        await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);
            
        var random = new Random();
        var files = Directory.GetFiles("Files");
        var randomPepeFilePath = files[random.Next(files.Length)];

        await using FileStream fileStream = new(randomPepeFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var fileName = randomPepeFilePath.Split(Path.DirectorySeparatorChar).Last();

        return await _botClient.SendPhotoAsync(message.Chat.Id,
            new InputOnlineFile(fileStream, fileName),
            $"Держи, вот твой Пепе");
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