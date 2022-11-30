namespace Speak.TelegramBot.Entities;

internal class TodayPepe
{
    public TodayPepe(string username, string pepeImageFilePath)
    {
        Username = username;
        PepeImageFilePath = pepeImageFilePath;
        WhenChosen = DateTimeOffset.Now;
    }

    public string Username { get; }
    
    public string PepeImageFilePath { get; }
    
    public DateTimeOffset WhenChosen { get; }
}