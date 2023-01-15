namespace Speak.Telegram.MusicQuizFeatureContracts;

public class StartMusicQuizFeatureRequest
{
    public StartMusicQuizFeatureRequest(long chatId, string? playerUsername)
    {
        ChatId = chatId;
        PlayerUsername = playerUsername;
    }

    public long ChatId { get; }
    public string? PlayerUsername { get; }
}