using Telegram.Bot.Types;

namespace Speak.Telegram.Bot;

internal interface IMessageHandlerFactory
{
    Task<Message>? GetHandlerFor(Message message, CancellationToken ct);
}