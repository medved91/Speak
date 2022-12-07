namespace Speak.Telegram.CutieFeature.Contracts;

#nullable disable

/// <summary>
/// Фраза, которая отправляется перед сообщением с лапусечкой и ее миссией
/// </summary>
public class CutieThinkingPhrase
{
    /// <summary>
    /// Текст фразы
    /// </summary>
    public string Phrase { get; set; }
}