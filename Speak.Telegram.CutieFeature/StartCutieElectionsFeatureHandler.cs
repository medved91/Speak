using Microsoft.EntityFrameworkCore;
using Speak.Telegram.CommonContracts;
using Speak.Telegram.CutieFeature.Contracts;
using Speak.Telegram.CutieFeature.Contracts.Requests;
using Speak.Telegram.Postgres;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Speak.Telegram.CutieFeature;

internal class StartCutieElectionsFeatureHandler : ITelegramFeatureHandler<StartCutieElectionsFeatureRequest, Message>
{
    private readonly TelegramBotDbContext _dbContext;
    private readonly ITelegramBotClient _botClient;
    
    public StartCutieElectionsFeatureHandler(TelegramBotDbContext dbContext, ITelegramBotClient botClient)
    {
        _dbContext = dbContext;
        _botClient = botClient;
    }

    public async Task<Message> Handle(StartCutieElectionsFeatureRequest request, CancellationToken ct)
    {
        var lastChosenCutieInChat = await _dbContext.ChosenCuties
            .Include(c => c.Player)
            .Include(c => c.Mission)
            .OrderByDescending(c => c.WhenChosen)
            .FirstOrDefaultAsync(c => c.Player.ChatId == request.ChatId, ct);

        var endOfPreviousDay = new DateTimeOffset(DateTime.Now.Date.AddTicks(-1));
        if (lastChosenCutieInChat?.WhenChosen > endOfPreviousDay)
            return await SendCurrentCutieReminder(request.ChatId, lastChosenCutieInChat, ct);

        var chatPlayers = await _dbContext.CutiePlayers
            .Where(p => p.ChatId == request.ChatId)
            .ToArrayAsync(ct);

        if (!chatPlayers.Any())
            return await _botClient.SendTextMessageAsync(request.ChatId,
                "В этом чате нет зарегистрировавшихся в Лапусечке игроков(",
                cancellationToken: ct);

        var cutie = await ChooseAndSaveCutie(chatPlayers, lastChosenCutieInChat, ct);

        await SendThinkingPhraseAndWait(request.ChatId, ct);

        return await _botClient.SendTextMessageAsync(request.ChatId,
            $"Лапусечка у нас сегодня {cutie.Player.FirstName} {cutie.Player.LastName} " +
            $"(@{cutie.Player.TelegramUsername})\nЗадание для Лапусечки: {cutie.Mission.Description}",
            cancellationToken: ct);
    }

    private async Task<Message> SendCurrentCutieReminder(long chatId, ChosenCutie lastChosenCutieInChat, CancellationToken ct)
    {
        var endOfCurrentDay = new DateTimeOffset(DateTime.Now.Date.AddDays(1).AddTicks(-1));

        var timeToNextElections = endOfCurrentDay.Subtract(DateTimeOffset.Now);
        
        return await _botClient.SendTextMessageAsync(chatId,
            $"Лапусечка сегодня {lastChosenCutieInChat.Player.FirstName} " +
            $"{lastChosenCutieInChat.Player.LastName} (@{lastChosenCutieInChat.Player.TelegramUsername})\n" +
            $"Задание для Лапусечки: {lastChosenCutieInChat.Mission.Description}\n" +
            $"До следующих выборов: {(int)timeToNextElections.TotalHours}ч.",
            cancellationToken: ct);
    }

    private async Task<ChosenCutie> ChooseAndSaveCutie(CutiePlayer[] chatPlayers, ChosenCutie? lastChosenCutieInChat,
        CancellationToken ct)
    {
        // Если игроков больше одного, то на следующий день предыдущую лапусечку не выбираем
        if (lastChosenCutieInChat != null && chatPlayers.Length > 1)
            chatPlayers = chatPlayers
                .Where(p => p.TelegramUsername != lastChosenCutieInChat.Player.TelegramUsername)
                .ToArray();
        
        var random = new Random();
        var chosenPlayer = chatPlayers[random.Next(chatPlayers.Length)];

        var missions = await _dbContext.CutieMissions.ToArrayAsync(ct);

        if (!missions.Any()) throw new ApplicationException("В БД нет ни одной миссии");

        var missionForCutie = missions[random.Next(missions.Length)];

        var cutie = new ChosenCutie
        {
            Player = chosenPlayer,
            Mission = missionForCutie,
            WhenChosen = DateTimeOffset.Now
        };

        await _dbContext.ChosenCuties.AddAsync(cutie, ct);
        await _dbContext.SaveChangesAsync(ct);
        
        return cutie;
    }

    private async Task SendThinkingPhraseAndWait(long chatId, CancellationToken ct)
    {
        var random = new Random();

        var thinkingPhrases = await _dbContext.CutieThinkingPhrases.ToArrayAsync(ct);

        var thinkingPhrase = thinkingPhrases.Any()
            ? thinkingPhrases[random.Next(thinkingPhrases.Length)]
            : new CutieThinkingPhrase { Phrase = "Думаю..." };

        await _botClient.SendTextMessageAsync(chatId, thinkingPhrase.Phrase, cancellationToken: ct);
        
        Thread.Sleep(TimeSpan.FromSeconds(3));
    }
}