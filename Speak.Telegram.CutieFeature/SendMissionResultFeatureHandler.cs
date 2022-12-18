using Microsoft.EntityFrameworkCore;
using Speak.Telegram.CommonContracts;
using Speak.Telegram.CutieFeature.Contracts.Requests;
using Speak.Telegram.Postgres;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Speak.Telegram.CutieFeature;

internal class SendMissionResultFeatureHandler : ITelegramFeatureHandler<SendMissionResultFeatureRequest, Message>
{
    private readonly TelegramBotDbContext _dbContext;
    private readonly ITelegramBotClient _botClient;

    private readonly string[] _missionCompletePhrases = { 
        "Отлично!", "Красота!", "Замечательно!", "Потрясающе!", "Просто офигенно!", "Обалденно!", "Жжошь!", "Молорик!",
        "Очент хорошо!", "Вот это я понимаю!", "Класс!", "Балдеж!", "Принято!", 
        "👍", "👌", "🤟", "🤘", "🫀", "💪", "😍", "🔥", "🍓"
    };

    public SendMissionResultFeatureHandler(TelegramBotDbContext dbContext, ITelegramBotClient botClient)
    {
        _dbContext = dbContext;
        _botClient = botClient;
    }

    public async Task<Message> Handle(SendMissionResultFeatureRequest request, CancellationToken ct)
    {
        var lastChosenCutie = await _dbContext.ChosenCuties
            .Include(c => c.Player)
            .Include(c => c.Mission)
            .OrderByDescending(c => c.WhenChosen)
            .FirstOrDefaultAsync(c => c.Player.ChatId == request.ChatId, ct);
        
        if (lastChosenCutie == null || lastChosenCutie.ElectionMessageId != request.RepliedToBotMessageId) 
            return new Message();

        if(lastChosenCutie.Player.TelegramUsername != request.UsernameWhoSentPhoto) 
            return await _botClient.SendTextMessageAsync(request.ChatId,
                "Прости, но выполненное задание должен прислать Лапусечка, а не ты >:(",
                replyToMessageId: request.UserReplyMessageId,
                cancellationToken: ct);
        
        if(lastChosenCutie.MissionResultMessageId.HasValue)
            return await _botClient.SendTextMessageAsync(request.ChatId,
                $"@{lastChosenCutie.Player.TelegramUsername} Ты уже выполнил задание, вот:",
                replyToMessageId: lastChosenCutie.MissionResultMessageId,
                cancellationToken: ct);

        lastChosenCutie.MissionResultMessageId = request.UserReplyMessageId;

        await _dbContext.SaveChangesAsync(ct);

        var random = new Random();
        return await _botClient.SendTextMessageAsync(request.ChatId,
            _missionCompletePhrases[random.Next(_missionCompletePhrases.Length)],
            replyToMessageId: request.UserReplyMessageId,
            cancellationToken: ct);
    }
}