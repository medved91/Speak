namespace Speak.WebRtc.WebRtcEntities;

public class Room
{
    public Room(string roomId)
    {
        RoomId = roomId;
        UsersInRoom = new List<User>();
        ChatMessages = new List<ChatMessage>();
    }
    
    public string RoomId { get; }
    
    public List<User> UsersInRoom { get; }
    
    public List<ChatMessage> ChatMessages { get; }
}