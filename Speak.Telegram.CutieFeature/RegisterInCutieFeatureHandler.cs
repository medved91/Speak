using Microsoft.EntityFrameworkCore;
using Speak.Telegram.CommonContracts;
using Speak.Telegram.CutieFeature.Contracts;
using Speak.Telegram.CutieFeature.Contracts.Requests;
using Speak.Telegram.Postgres;
using Telegram.Bot;
using Telegram.Bot.Types;
using Chat = Speak.Telegram.CommonContracts.Chat;

namespace Speak.Telegram.CutieFeature;

internal class RegisterInCutieFeatureHandler : ITelegramFeatureHandler<RegisterInCutieFeatureRequest, Message>
{
    private readonly TelegramBotDbContext _dbContext;
    private readonly ITelegramBotClient _botClient;

    public RegisterInCutieFeatureHandler(TelegramBotDbContext context, ITelegramBotClient botClient)
    {
        _dbContext = context;
        _botClient = botClient;
    }

    public async Task<Message> Handle(RegisterInCutieFeatureRequest request, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(request.Username))
            return await _botClient.SendTextMessageAsync(request.ChatId, 
                "Прости, я что-то не могу разобрать твое имя пользователя...", 
                cancellationToken: ct);

        var alreadyRegisteredPlayer = await _dbContext.CutiePlayers
            .FirstOrDefaultAsync(p => 
                p.Chat.TelegramChatId == request.ChatId && p.TelegramUsername == request.Username, ct);

        if (alreadyRegisteredPlayer != null)
            return await _botClient.SendTextMessageAsync(request.ChatId, "Ты уже участвуешь в выборах Лапусечки",
                replyToMessageId: request.InitialMessageId, cancellationToken: ct);

        var chat = await GetOrAddChat(request, ct);

        var newPlayer = new CutiePlayer
        {
            TelegramUsername = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Chat = chat
        };

        await _dbContext.CutiePlayers.AddAsync(newPlayer, ct);
        await _dbContext.SaveChangesAsync(ct);
        
        return await _botClient.SendTextMessageAsync(request.ChatId, 
            "Подравляю, ты добавлен в участники ежедневных выборов Лапусечки!",
            replyToMessageId: request.InitialMessageId, cancellationToken: ct);
    }

    private async Task<Chat> GetOrAddChat(RegisterInCutieFeatureRequest request, CancellationToken ct)
    {
        var chat = await _dbContext.Chats.FirstOrDefaultAsync(c => c.TelegramChatId == request.ChatId, ct);
        if (chat != null) return chat;
        
        chat = (await _dbContext.Chats.AddAsync(new Chat { TelegramChatId = request.ChatId }, ct)).Entity;
        await _dbContext.SaveChangesAsync(ct);

        return chat;
    }
}