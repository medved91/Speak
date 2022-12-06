namespace Speak.Telegram.CutieFeature;

#nullable disable

/// <summary>
/// Выбранная Лапусечка
/// </summary>
public class Cutie
{
    /// <summary>
    /// Пользователь, выбранный лапусечкой
    /// </summary>
    public Player Player { get; set; }

    /// <summary>
    /// Задание для Лапусечки
    /// </summary>
    public Mission Mission { get; set; }
    
    /// <summary>
    /// Когда был выбран
    /// </summary>
    public DateTimeOffset WhenChosen { get; set; }
}