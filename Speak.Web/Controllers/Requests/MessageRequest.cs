namespace Speak.Web.Controllers.Requests;

public class MessageRequest
{
    public long ChannelId { get; set; }
    
    public string Message { get; set; }
    
    public string UserNameToTag { get; set; }
}