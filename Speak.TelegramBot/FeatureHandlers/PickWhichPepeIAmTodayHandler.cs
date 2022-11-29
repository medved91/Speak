using Speak.TelegramBot.FeatureRequests;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace Speak.TelegramBot.FeatureHandlers;

internal class PickWhichPepeIAmTodayHandler : ITelegramFeatureHandler<PickWhichPepeIAmTodayRequest, Message>
{
    private readonly ITelegramBotClient _botClient;

    public PickWhichPepeIAmTodayHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task<Message> Handle(PickWhichPepeIAmTodayRequest request)
    {
        return await SendPepe(request.ChatId);
    }
    
    private async Task<Message> SendPepe(long chatId)
    {
        await _botClient.SendChatActionAsync(chatId, ChatAction.UploadPhoto);
            
        var random = new Random();
        var files = Directory.GetFiles("Files");
        var randomPepeFilePath = files[random.Next(files.Length)];

        await using FileStream fileStream = new(randomPepeFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var fileName = randomPepeFilePath.Split(Path.DirectorySeparatorChar).Last();

        return await _botClient.SendPhotoAsync(chatId,
            new InputOnlineFile(fileStream, fileName),
            "Держи, вот твой Пепе");
    }
}