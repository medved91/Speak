namespace Speak.Telegram.AudioContracts;

public interface IAudioService
{
    MemoryStream GetShortenedAudioFileStream(string audioFilePath);
    
    AudioFileTags GetAudioFileTags(string audioFilePath);
}