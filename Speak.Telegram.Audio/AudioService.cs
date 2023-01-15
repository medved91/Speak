using NAudio.Wave;
using NLayer.NAudioSupport;
using Speak.Telegram.AudioContracts;

namespace Speak.Telegram.Audio;

internal class AudioService : IAudioService
{
    public MemoryStream GetShortenedAudioFileStream(string randomAudioFilePath)
    {
        var builder = new Mp3FileReaderBase.FrameDecompressorBuilder(wf => new Mp3FrameDecompressor(wf));
        var audioFile = new Mp3FileReaderBase(randomAudioFilePath, builder);
        var audioFileLength = audioFile.TotalTime;

        var random = new Random();
        var trimmed = audioFile
            .ToSampleProvider()
            .Skip(TimeSpan.FromSeconds(random.Next((int)(audioFileLength.TotalSeconds/4), (int)(audioFileLength.TotalSeconds/1.4))))
            .Take(TimeSpan.FromSeconds(5))
            .ToWaveProvider16();
        
        var stream = new MemoryStream();
        WaveFileWriter.WriteWavFileToStream(stream, trimmed);

        return stream;
    }

    public AudioFileTags GetAudioFileTags(string audioFilePath)
    {
        var tags = TagLib.File.Create(audioFilePath);
        
        return new AudioFileTags(tags.Tag.FirstPerformer, tags.Tag.Title);
    }
}