namespace Speak.TelegramBot.FeatureRequests;

internal class PickWhichPepeIAmTodayRequest
{
    public PickWhichPepeIAmTodayRequest(long chatId)
    {
        ChatId = chatId;
    }

    /// <summary>
    /// Идентификатор чата, в который будет выслан пепе
    /// </summary>
    public long ChatId { get; }
}