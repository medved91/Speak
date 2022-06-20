namespace Speak.Web;

public class Room
{
    public Room(string roomId)
    {
        RoomId = roomId;
        UsersInRoom = new List<User>();
    }
    
    public string RoomId { get; }
    
    public List<User> UsersInRoom { get; }
}