namespace Speak.Telegram.CutieFeature.Contracts;

#nullable disable

/// <summary>
/// Выбранная Лапусечка
/// </summary>
public class ChosenCutie
{
    /// <summary>
    /// Пользователь, выбранный лапусечкой
    /// </summary>
    public CutiePlayer Player { get; set; }

    /// <summary>
    /// Задание для Лапусечки
    /// </summary>
    public CutieMission Mission { get; set; }
    
    /// <summary>
    /// Когда был выбран
    /// </summary>
    public DateTimeOffset WhenChosen { get; set; }
}