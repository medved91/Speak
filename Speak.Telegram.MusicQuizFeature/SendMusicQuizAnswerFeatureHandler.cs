using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Speak.Telegram.CommonContracts;
using Speak.Telegram.MusicQuizFeatureContracts;
using Speak.Telegram.Postgres;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Speak.Telegram.MusicQuizFeature;

internal class SendMusicQuizAnswerFeatureHandler : ITelegramFeatureHandler<SendMusicQuizAnswerFeatureRequest, Message>
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<SendMusicQuizAnswerFeatureHandler> _logger;
    private readonly TelegramBotDbContext _dbContext;
    
    public SendMusicQuizAnswerFeatureHandler(ITelegramBotClient botClient, 
        TelegramBotDbContext dbContext, 
        ILogger<SendMusicQuizAnswerFeatureHandler> logger)
    {
        _botClient = botClient;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Message> Handle(SendMusicQuizAnswerFeatureRequest request, CancellationToken ct)
    {
        if(request.ChatId == null)
        {
            await _botClient.AnswerCallbackQueryAsync(request.CallbackId, 
                "Где-то потерялся идентификатор твоего чата и я не могу дальше работать :( Напиши @medveden91",
                cancellationToken: ct);

            return new Message();
        }

        if (request.MessageIdWithButtonPushed == null)
        {
            await _botClient.AnswerCallbackQueryAsync(request.CallbackId, 
                "Где-то потерялся идентификатор сообщения с игрой и я не могу дальше работать :( Напиши @medveden91",
                cancellationToken: ct);

            return new Message();
        }

        if (request.PlayerUsername == null)
        {
            await _botClient.AnswerCallbackQueryAsync(request.CallbackId, 
                "Где-то потерялся твой логин и я не могу дальше работать :( Напиши @medveden91",
                cancellationToken: ct);

            return new Message();
        }
        
        var musicQuizRound = await _dbContext.MusicQuizRounds.FirstOrDefaultAsync(
            r => r.Chat.TelegramChatId == request.ChatId 
                 && r.RoundMessageId == request.MessageIdWithButtonPushed, cancellationToken: ct);

        if (musicQuizRound == null)
        {
            await _botClient.AnswerCallbackQueryAsync(request.CallbackId, 
                "Хз, на что ты нажал. Я не могу найти это сообщение. Напиши @medveden91",
                cancellationToken: ct);

            return new Message();
        }

        if (musicQuizRound.PlayerUsername != request.PlayerUsername)
        {
            await _botClient.AnswerCallbackQueryAsync(request.CallbackId, 
                "Ты не можешь отвечать на чужой квиз!",
                cancellationToken: ct);

            return new Message();
        }
        
        if(musicQuizRound.AnsweredCorrectly.HasValue)
        {
            var answerMessage = musicQuizRound.AnsweredCorrectly.Value
                ? "Ты уже отвечал. В тот раз ты ответил правильно, молодец!"
                : "Ты уже отвечат и ответил ты неправильно";
            
            await _botClient.AnswerCallbackQueryAsync(request.CallbackId, $"{answerMessage}",
                cancellationToken: ct);

            return new Message();
        }

        if (request.Answer == AnswerCallback.Correct)
        {
            musicQuizRound.AnsweredCorrectly = true;
            await _dbContext.SaveChangesAsync(ct);
            
            await _botClient.AnswerCallbackQueryAsync(request.CallbackId, "👍",
                cancellationToken: ct);
            
            return await _botClient.SendTextMessageAsync(request.ChatId, 
                $"@{request.PlayerUsername} Молорик! Отвечено правильно! Это {musicQuizRound.Artist} - {musicQuizRound.Title}",
                cancellationToken: ct);
        }

        if(request.Answer == AnswerCallback.Wrong)
        {
            musicQuizRound.AnsweredCorrectly = false;
            await _dbContext.SaveChangesAsync(ct);
            
            await _botClient.AnswerCallbackQueryAsync(request.CallbackId, "👎",
                cancellationToken: ct);
        
            return await _botClient.SendTextMessageAsync(request.ChatId, 
                $"@{request.PlayerUsername} Ппц, как тут можно было ошибиться? Правильный ответ был: {musicQuizRound.Artist} - {musicQuizRound.Title}",
                cancellationToken: ct);
        }
        
        await _botClient.AnswerCallbackQueryAsync(request.CallbackId, 
            "Я хз, на что ты нажал. Произошла какая-то фигня и я не могу обработать твой ответ. Напиши @medveden91",
            cancellationToken: ct);

        return new Message();
    }
}