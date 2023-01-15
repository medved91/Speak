using Telegram.Bot.Types;

namespace Speak.Telegram.Bot;

internal interface IMessageHandlerFactory
{
    /// <summary>
    /// Получить хендлер для обработки сообщений
    /// </summary>
    Task<Message>? GetHandlerFor(Message message, CancellationToken ct);
    
    /// <summary>
    /// Получить хендлер для обработки Callback
    /// </summary>
    Task<Message>? GetHandlerFor(CallbackQuery callback, CancellationToken ct);
}