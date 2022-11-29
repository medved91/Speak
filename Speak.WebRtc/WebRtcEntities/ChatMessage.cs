namespace Speak.WebRtc.WebRtcEntities;

public class ChatMessage
{
    public ChatMessage(User fromUser, string messageText)
    {
        FromUser = fromUser;
        MessageText = messageText;
        PostedAt = DateTime.UtcNow;
    }

    public User FromUser { get; }
    
    public string MessageText { get; }
    
    public DateTime PostedAt { get; }
}