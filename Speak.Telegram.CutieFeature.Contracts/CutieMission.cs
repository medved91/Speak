namespace Speak.Telegram.CutieFeature.Contracts;

#nullable disable

/// <summary>
/// Задание для лапусечки
/// </summary>
public class CutieMission
{
    /// <summary>
    /// Идентификатор задания
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Текст задания
    /// </summary>
    public string Description { get; set; }
}