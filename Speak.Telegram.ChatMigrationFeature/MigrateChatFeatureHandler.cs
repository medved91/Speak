using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Speak.Telegram.ChatMigrationFeatureContracts;
using Speak.Telegram.CommonContracts;
using Speak.Telegram.Postgres;
using Telegram.Bot;
using Telegram.Bot.Types;
using Chat = Speak.Telegram.CommonContracts.Chat;

namespace Speak.Telegram.ChatMigrationFeature;

internal class MigrateChatFeatureHandler : ITelegramFeatureHandler<MigrateChatFeatureRequest, Message>
{
    private readonly ITelegramBotClient _botClient;
    private readonly TelegramBotDbContext _dbContext;
    private readonly ILogger<MigrateChatFeatureHandler> _logger;

    public MigrateChatFeatureHandler(ITelegramBotClient botClient, TelegramBotDbContext dbContext, 
        ILogger<MigrateChatFeatureHandler> logger)
    {
        _botClient = botClient;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Message> Handle(MigrateChatFeatureRequest request, CancellationToken ct)
    {
        if(request.NewChatId == null) 
        {
            _logger.LogError("Не удалось мигрировать чат c Id: {OldChatId}. Из телеграма не пришел новый Id", 
                request.OldChatId);
            
            return await _botClient.SendTextMessageAsync(request.OldChatId,
                "У текущей группы сменился идентификатор. " +
                "В процессе миграции настроек произошла ошибка :( Напиши, пожалуйста, @medveden91", 
                cancellationToken: ct);
        }

        var chatToMigrate = await _dbContext.Chats
            .FirstOrDefaultAsync(c => c.TelegramChatId == request.OldChatId, ct);

        if (chatToMigrate == null)
        {
            var newChat = new Chat { TelegramChatId = request.NewChatId.Value };
            _dbContext.Chats.Add(newChat);
            await _dbContext.SaveChangesAsync(ct);
            
            _logger.LogInformation(
                "Не удалось мигрировать чат с Id: {OldChatId}. " + 
                "Чат не найден в нашей БД. Добавили чат с новым Id: {NewChatId}", 
                request.OldChatId, request.NewChatId);

            return await _botClient.SendTextMessageAsync(request.OldChatId,
                "У текущей группы сменился идентификатор. Настройки промигрированы успешно", cancellationToken: ct);
        }

        chatToMigrate.TelegramChatId = request.NewChatId.Value;

        await _dbContext.SaveChangesAsync(ct);

        var resultMessage = await _botClient.SendTextMessageAsync(chatToMigrate.TelegramChatId,
            "У текущей группы сменился идентификатор. Настройки промигрированы успешно", cancellationToken: ct);
        
        _logger.LogInformation("Промигрирован чат. Старый Id: {OldChatId}. Новый Id: {NewChatId}", 
            request.OldChatId, request.NewChatId);

        return resultMessage;
    }
}