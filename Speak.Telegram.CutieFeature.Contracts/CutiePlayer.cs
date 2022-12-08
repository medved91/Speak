namespace Speak.Telegram.CutieFeature.Contracts;


/// <summary>
/// Зарегистрировавшийся игрок в Лапусечку
/// </summary>
public class CutiePlayer
{
    /// <summary>
    /// Имя пользователя в Телеграм
    /// </summary>
    public string TelegramUsername { get; set; } = null!;

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string? FirstName { get; set; }
    
    /// <summary>
    /// Фамилия пользователя
    /// </summary>
    public string? LastName { get; set; }
    
    /// <summary>
    /// На каком канале пользователь зарегался в игре
    /// </summary>
    public long ChatId { get; set; }
}