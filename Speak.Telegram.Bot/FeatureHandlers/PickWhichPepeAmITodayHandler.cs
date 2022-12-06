using Speak.Storage;
using Speak.Telegram.Bot.Entities;
using Speak.Telegram.Bot.FeatureRequests;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace Speak.Telegram.Bot.FeatureHandlers;

internal class PickWhichPepeAmITodayHandler : ITelegramFeatureHandler<PickWhichPepeAmITodayRequest, Message>
{
    private readonly IRepository<TodayPepe> _todayPepesRepo;
    private readonly ITelegramBotClient _botClient;

    private static readonly string[] ThinkingPhrases = {
        "Дай-ка на тебя глянуть...",
        "Анализирую выражение твоего лица...",
        "Секундочку, мне надо подумать...",
        "Считываю твое настроение...",
        "Анализирую, как ты себя вел последние дни..."
    };

    public PickWhichPepeAmITodayHandler(ITelegramBotClient botClient, IRepository<TodayPepe> todayPepesRepo)
    {
        _botClient = botClient;
        _todayPepesRepo = todayPepesRepo;
    }

    public async Task<Message> Handle(PickWhichPepeAmITodayRequest request)
    {
        if (string.IsNullOrEmpty(request.Username))
            return await _botClient.SendTextMessageAsync(request.ChatId, 
                "Прости, я что-то не могу разобрать твое имя пользователя...");
        
        await _botClient.SendChatActionAsync(request.ChatId, ChatAction.ChooseSticker);

        var currentUserPepe = await GetCurrentUserAlreadyChosenPepe(request.Username);
        if (currentUserPepe != null) 
            return await SendPepeAsync(request.ChatId,
                request.InitialMessageId,
                currentUserPepe.PepeImageFilePath,
                "Подзабыл?) Сегодня ты такой Пепе");

        var random = new Random();
        var randomPhrase = ThinkingPhrases[random.Next(ThinkingPhrases.Length)];
        await _botClient.SendTextMessageAsync(request.ChatId, randomPhrase);
        Thread.Sleep(4000);
        
        return await PickPepeAndSendMessage(request);
    }

    internal async Task<TodayPepe?> GetCurrentUserAlreadyChosenPepe(string username)
    {
        var endOfPreviousDay = new DateTimeOffset(DateTime.Now.Date.AddTicks(-1));
        
        var currentUserPepe = await _todayPepesRepo
            .FirstOrDefaultAsync(p => p.Username == username
                && p.WhenChosen > endOfPreviousDay);
        
        return currentUserPepe;
    }

    private async Task<Message> PickPepeAndSendMessage(PickWhichPepeAmITodayRequest request)
    {
        var random = new Random();
        var files = Directory.GetFiles("Files");
        var randomPepeFilePath = files[random.Next(files.Length)];

        var todayUserPepe = new TodayPepe(request.Username!, randomPepeFilePath);
        await _todayPepesRepo.AddAsync(todayUserPepe);

        return await SendPepeAsync(request.ChatId,
            request.InitialMessageId,
            randomPepeFilePath,
            "Сегодня ты вот такой Пепе)");
    }

    private async Task<Message> SendPepeAsync(long chatId, int replyToMessageId, string filePath, string replyMessage)
    {
        await using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

        var file = new InputOnlineFile(fileStream, fileName);
        
        if(fileName.EndsWith(".gif"))
            return await _botClient.SendAnimationAsync(chatId, file, caption:replyMessage, 
                replyToMessageId:replyToMessageId);

        return await _botClient.SendPhotoAsync(chatId, file, replyMessage, replyToMessageId:replyToMessageId);
    }
}