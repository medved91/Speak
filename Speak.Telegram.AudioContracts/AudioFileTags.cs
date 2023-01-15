namespace Speak.Telegram.AudioContracts;

/// <summary>
/// Теги аудио-файла
/// </summary>
public class AudioFileTags
{
    public AudioFileTags(string artist, string title)
    {
        Artist = artist;
        Title = title;
    }

    /// <summary>
    /// Исполнитель
    /// </summary>
    public string Artist { get; }
    
    /// <summary>
    /// Название трека
    /// </summary>
    public string Title { get; }
}