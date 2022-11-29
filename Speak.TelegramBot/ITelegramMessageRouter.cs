using Telegram.Bot.Types;

namespace Speak.TelegramBot;

/// <summary>
/// Сервис, распределяющий по фичам полученные из Телеги сообщения 
/// </summary>
public interface ITelegramMessageRouter
{
    Task HandleNewMessageAsync(Update update);
}