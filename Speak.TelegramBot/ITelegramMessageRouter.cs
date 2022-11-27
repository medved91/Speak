using Telegram.Bot.Types;

namespace Speak.TelegramBot;

public interface ITelegramMessageRouter
{
    Task HandleNewMessageAsync(Update update);
}