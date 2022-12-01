namespace Speak.TelegramBot.Entities;

internal class TodayPepe
{
    public TodayPepe(string username, string pepeImageFilePath, DateTimeOffset whenChosen = default)
    {
        Username = username;
        PepeImageFilePath = pepeImageFilePath;
        
        if(whenChosen == default)
            WhenChosen = DateTimeOffset.Now;
    }

    public string Username { get; }
    
    public string PepeImageFilePath { get; }
    
    public DateTimeOffset WhenChosen { get; }
}