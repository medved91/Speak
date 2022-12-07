using Speak.Telegram.CommonContracts;
using Speak.Telegram.CutieFeature.Contracts;
using Speak.Telegram.CutieFeature.Contracts.Requests;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Speak.Telegram.CutieFeature;

internal class RegisterInCutieFeatureHandler : ITelegramFeatureHandler<RegisterInCutieFeatureRequest, Message>
{
    private readonly ICutieRepository _repository;
    private readonly ITelegramBotClient _botClient;

    public RegisterInCutieFeatureHandler(ICutieRepository repository, ITelegramBotClient botClient)
    {
        _repository = repository;
        _botClient = botClient;
    }

    public async Task<Message> Handle(RegisterInCutieFeatureRequest request, CancellationToken ct)
    {
        var alreadyRegisteredPlayer = await _repository.Players
            .FirstOrDefaultAsync(p => p.ChatId == request.ChatId && p.TelegramUsername == request.Username, ct);

        if (alreadyRegisteredPlayer != null)
            return await _botClient.SendTextMessageAsync(request.ChatId, "Ты уже участвуешь в выборах Лапусечки",
                replyToMessageId: request.InitialMessageId, cancellationToken: ct);

        var newPlayer = new CutiePlayer
        {
            TelegramUsername = request.Username,
            ChatId = request.ChatId
        };

        await _repository.Players.AddAsync(newPlayer, ct);
        return await _botClient.SendTextMessageAsync(request.ChatId, 
            "Подравляю, ты добавлен в участники ежедневных выборов Лапусечки!",
            replyToMessageId: request.InitialMessageId, cancellationToken: ct);
    }
}