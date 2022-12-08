namespace Speak.Telegram.PepeFeature;

public class PickWhichPepeAmITodayFeatureRequest
{
    public PickWhichPepeAmITodayFeatureRequest(string? username, long chatId, int initialMessageId)
    {
        Username = username;
        ChatId = chatId;
        InitialMessageId = initialMessageId;
    }

    /// <summary>
    /// Никнейм (логин) пользователя, которому выбираем пепе
    /// </summary>
    public string? Username { get; }

    /// <summary>
    /// Идентификатор чата, в который будет выслан пепе
    /// </summary>
    public long ChatId { get; }

    /// <summary>
    /// Идентификатор сообщения, которое запустило выбор Пепе
    /// </summary>
    public int InitialMessageId { get; }
}