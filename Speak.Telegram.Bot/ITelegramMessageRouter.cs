using Telegram.Bot.Types;

namespace Speak.Telegram.Bot;

/// <summary>
/// Сервис, распределяющий по фичам полученные из Телеги сообщения 
/// </summary>
public interface ITelegramMessageRouter
{
    Task HandleNewMessageAsync(Update update);
}