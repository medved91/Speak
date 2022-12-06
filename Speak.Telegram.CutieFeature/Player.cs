namespace Speak.Telegram.CutieFeature;

#nullable disable

/// <summary>
/// Зарегистрировавшийся игрок в Лапусечку
/// </summary>
public class Player
{
    /// <summary>
    /// Имя пользователя в Телеграм
    /// </summary>
    public string TelegramUsername { get; set; }
    
    /// <summary>
    /// На каком канале пользователь зарегался в игре
    /// </summary>
    public long ChatId { get; set; }
}