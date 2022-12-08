namespace Speak.Telegram.CutieFeature.Contracts.Requests;

/// <summary>
/// Реквест на выполнение фичи выборов Лапусечки
/// </summary>
public class StartCutieElectionsFeatureRequest
{
    public StartCutieElectionsFeatureRequest(long chatId)
    {
        ChatId = chatId;
    }

    /// <summary>
    /// Идентификатор чата, в котором запускаются выборы
    /// </summary>
    public long ChatId { get; }
}