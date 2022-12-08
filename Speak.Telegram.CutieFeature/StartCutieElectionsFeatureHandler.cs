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
        var endOfPreviousDay = new DateTimeOffset(DateTime.Now.Date.AddTicks(-1));

        var lastChosenCutieInChat = await _dbContext.ChosenCuties
            .Include(c => c.Player)
            .Include(c => c.Mission)
            .OrderByDescending(c => c.WhenChosen)
            .FirstOrDefaultAsync(c => c.Player.ChatId == request.ChatId, ct);

        if (lastChosenCutieInChat?.WhenChosen > endOfPreviousDay)
            return await _botClient.SendTextMessageAsync(request.ChatId,
                $"Сегодняшний Лапусечка уже выбран! Это {lastChosenCutieInChat.Player.FirstName} " +
                $"{lastChosenCutieInChat.Player.LastName} (@{lastChosenCutieInChat.Player.TelegramUsername}) " +
                $"и его задание: {lastChosenCutieInChat.Mission.Description}",
                cancellationToken: ct);

        var chatPlayers = await _dbContext.CutiePlayers
            .Where(p => p.ChatId == request.ChatId)
            .ToArrayAsync(ct);

        if (!chatPlayers.Any())
            return await _botClient.SendTextMessageAsync(request.ChatId,
                "В этом чате нет зарегистрировавшихся в Лапусечке игроков(",
                cancellationToken: ct);

        var cutie = await ChooseAndSaveCutie(chatPlayers, ct);

        await SendThinkingPhraseAndWait(request.ChatId, ct);

        return await _botClient.SendTextMessageAsync(request.ChatId,
            $"Лапусечка у нас сегодня {cutie.Player.FirstName} {cutie.Player.LastName} " +
            $"(@{cutie.Player.TelegramUsername}), задание для Лапусечки: {cutie.Mission.Description}",
            cancellationToken: ct);
    }

    private async Task<ChosenCutie> ChooseAndSaveCutie(CutiePlayer[] chatPlayers, CancellationToken ct)
    {
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