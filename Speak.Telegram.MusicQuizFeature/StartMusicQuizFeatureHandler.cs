using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Speak.Extensions;
using Speak.Telegram.AudioContracts;
using Speak.Telegram.CommonContracts;
using Speak.Telegram.MusicQuizFeatureContracts;
using Speak.Telegram.Postgres;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Chat = Speak.Telegram.CommonContracts.Chat;

namespace Speak.Telegram.MusicQuizFeature;

internal class StartMusicQuizFeatureHandler : ITelegramFeatureHandler<StartMusicQuizFeatureRequest, Message>
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<StartMusicQuizFeatureHandler> _logger;
    private readonly IAudioService _audioService;
    private readonly TelegramBotDbContext _dbContext;

    private const string MusicFolderPath = "/app/Music";

    public StartMusicQuizFeatureHandler(ITelegramBotClient botClient, 
        IAudioService audioService, 
        TelegramBotDbContext dbContext, 
        ILogger<StartMusicQuizFeatureHandler> logger)
    {
        _botClient = botClient;
        _logger = logger;
        _dbContext = dbContext;
        _audioService = audioService;
    }

    public async Task<Message> Handle(StartMusicQuizFeatureRequest request, CancellationToken ct)
    {
        if(string.IsNullOrEmpty(request.PlayerUsername))
            return await _botClient.SendTextMessageAsync(request.ChatId,
                "Сорян, для запуска `Угадай мелодию` тебе нужен логин, а у тебя его почему-то нет :(", 
                cancellationToken: ct);
        
        var randomAudioFilesPaths = GetRandomAudioFilesPaths(4);

        if (randomAudioFilesPaths.Length < 2)
        {
            _logger.LogWarning("Для запуска фичи Угадай мелодию недостаточно аудиозаписей");
            return await _botClient.SendTextMessageAsync(request.ChatId,
                "Сорян, для запуска `Угадай мелодию` недостаточно аудиозаписей в боте :( " +
                "Напиши, пожалуйста, @medveden91", cancellationToken: ct);
        }

        var chat = await GetOrAddChat(request, ct);

        var unfinishedMusicQuizRound = await _dbContext.MusicQuizRounds.FirstOrDefaultAsync(
            r => r.Chat.TelegramChatId == request.ChatId 
                 && r.PlayerUsername == request.PlayerUsername 
                 && r.AnsweredCorrectly == null, cancellationToken: ct);

        if (unfinishedMusicQuizRound != null)
            return await _botClient.SendTextMessageAsync(request.ChatId, 
                $"@{request.PlayerUsername} Ты еще не угадал прошлую песню!",
                replyToMessageId: unfinishedMusicQuizRound.RoundMessageId, 
                cancellationToken: ct);

        var correctAnswerFilePath = randomAudioFilesPaths.First();

        var quizMessage = await SendQuizMessage(request.ChatId, correctAnswerFilePath, randomAudioFilesPaths.Skip(1), ct);

        var correctAnswerTags = _audioService.GetAudioFileTags(correctAnswerFilePath);
        
        var musicQuizRound = new MusicQuizRound
        {
            Chat = chat,
            PlayerUsername = request.PlayerUsername,
            RoundMessageId = quizMessage.MessageId,
            StartedAt = DateTimeOffset.UtcNow,
            Artist = correctAnswerTags.Artist,
            Title = correctAnswerTags.Title
        };

        _dbContext.MusicQuizRounds.Add(musicQuizRound);
        await _dbContext.SaveChangesAsync(ct);

        return quizMessage;
    }

    private async Task<Message> SendQuizMessage(long chatId, string correctAnswerFilePath,
        IEnumerable<string> incorrectAnswersFilesPaths, CancellationToken ct)
    {
        using var shortAudioStream = _audioService.GetShortenedAudioFileStream(correctAnswerFilePath);
        shortAudioStream.Seek(0, SeekOrigin.Begin);

        var answers = GetAnswerButtons(correctAnswerFilePath, incorrectAnswersFilesPaths);

        var quizMessage = await _botClient.SendAudioAsync(chatId,
            new InputOnlineFile(shortAudioStream, "Держи, послушай"),
            "Как думаешь, что за песня играет?",
            duration: 5,
            replyMarkup: new InlineKeyboardMarkup(answers),
            cancellationToken: ct);
        
        return quizMessage;
    }

    private async Task<Chat> GetOrAddChat(StartMusicQuizFeatureRequest request, CancellationToken ct)
    {
        var chat = await _dbContext.Chats
            .FirstOrDefaultAsync(c => c.TelegramChatId == request.ChatId, cancellationToken: ct);

        if (chat == null)
        {
            chat = _dbContext.Chats.Add(new Chat { TelegramChatId = request.ChatId }).Entity;
            await _dbContext.SaveChangesAsync(ct);
        }

        return chat;
    }

    private string[] GetRandomAudioFilesPaths(int count)
    {
        var audioFilesPaths = Directory.GetFiles(MusicFolderPath).ToList();
        
        _logger.LogInformation("Найдено {FilesCount} аудиофайлов", audioFilesPaths.Count);

        var result = new List<string>(count);
        
        var random = new Random();

        for (var i = 0; i < count; i++)
        {
            var randomIndex = random.Next(audioFilesPaths.Count);
            var randomAudioFilePath = audioFilesPaths[randomIndex];
            result.Add(randomAudioFilePath);
            audioFilesPaths.RemoveAt(randomIndex);
        }

        return result.ToArray();
    }

    private InlineKeyboardButton[][] GetAnswerButtons(string correctAnswerPath, IEnumerable<string> incorrectAnswerPaths)
    {
        var correctAnswerTags = _audioService.GetAudioFileTags(correctAnswerPath);

        var answers = new List<InlineKeyboardButton[]>
        {
            new InlineKeyboardButton[]
            {
                new ($"{correctAnswerTags.Artist} - {correctAnswerTags.Title}") { CallbackData = AnswerCallback.Correct.ToString() }
            }
        };

        var incorrectAnswers = incorrectAnswerPaths
            .Select(f =>
            {
                var audioFileTags = _audioService.GetAudioFileTags(f);

                return new InlineKeyboardButton[]
                {
                    new($"{audioFileTags.Artist} - {audioFileTags.Title}") { CallbackData = AnswerCallback.Wrong.ToString() }
                };
            });

        answers.AddRange(incorrectAnswers);

        var answersArray = answers.ToArray();
        answersArray.Shuffle();
        
        return answersArray;
    }
}