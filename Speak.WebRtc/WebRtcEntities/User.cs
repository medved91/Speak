namespace Speak.WebRtc.WebRtcEntities;

public class User
{
    public User(string connectionId)
    {
        ConnectionId = connectionId;
    }
    
    public string ConnectionId { get; }

    public string Name { get; set; } = "";
}