namespace Speak.Telegram.CutieFeature.Contracts.Requests;

/// <summary>
/// Запрос на регистрацию в игре Лапусечка
/// </summary>
public class RegisterInCutieFeatureRequest
{
    public RegisterInCutieFeatureRequest(string? username, long chatId, int initialMessageId)
    {
        Username = username;
        ChatId = chatId;
        InitialMessageId = initialMessageId;
    }

    /// <summary>
    /// Идентификатор чата, в котором регистрируем участника
    /// </summary>
    public long ChatId { get; }

    /// <summary>
    /// Имя пользователя, которого регистрируем
    /// </summary>
    public string? Username { get; }

    /// <summary>
    /// Идентификатор сообщения с командой на регистрацию, которое отослал пользователь
    /// </summary>
    public int InitialMessageId { get; }
}