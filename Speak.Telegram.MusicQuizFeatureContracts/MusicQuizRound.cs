using Speak.Telegram.CommonContracts;

namespace Speak.Telegram.MusicQuizFeatureContracts;

#nullable disable

/// <summary>
/// Раунд в игре Угадай мелодию
/// </summary>
public class MusicQuizRound
{
    /// <summary>
    /// Чат, в котором запущен раунд
    /// </summary>
    public Chat Chat { get; set; }
    
    /// <summary>
    /// Кто играет (логин)
    /// </summary>
    public string PlayerUsername { get; set; }
    
    /// <summary>
    /// Идентификатор сообщения с песней и вариантами
    /// </summary>
    public int RoundMessageId { get; set; }
    
    /// <summary>
    /// Получен правильный ответ (null если ответа не было)
    /// </summary>
    public bool? AnsweredCorrectly { get; set; }
    
    /// <summary>
    /// Когда стартовал этот раунд
    /// </summary>
    public DateTimeOffset StartedAt { get; set; }
    
    /// <summary>
    /// Исполнитель песни текущего раунда
    /// </summary>
    public string Artist { get; set; }
    
    /// <summary>
    /// Название песни текущего раунда
    /// </summary>
    public string Title { get; set; }
}