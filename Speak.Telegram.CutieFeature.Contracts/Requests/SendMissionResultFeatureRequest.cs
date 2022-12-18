namespace Speak.Telegram.CutieFeature.Contracts.Requests;

public class SendMissionResultFeatureRequest
{
    public SendMissionResultFeatureRequest(long chatId, 
        int repliedToBotMessageId, 
        string? usernameWhoSentPhoto, 
        int userReplyMessageId)
    {
        ChatId = chatId;
        RepliedToBotMessageId = repliedToBotMessageId;
        UsernameWhoSentPhoto = usernameWhoSentPhoto;
        UserReplyMessageId = userReplyMessageId;
    }

    /// <summary>
    /// Идентификатор чата, в котором прислали фотку
    /// </summary>
    public long ChatId { get; }
    
    /// <summary>
    /// Идентификатор сообщения, в ответ на которое послали фотку
    /// </summary>
    public int RepliedToBotMessageId { get; }
    
    /// <summary>
    /// Имя пользователя, который послал фотку
    /// </summary>
    public string? UsernameWhoSentPhoto { get; }
    
    /// <summary>
    /// Идентификатор сообщения с фоткой
    /// </summary>
    public int UserReplyMessageId { get; }
}