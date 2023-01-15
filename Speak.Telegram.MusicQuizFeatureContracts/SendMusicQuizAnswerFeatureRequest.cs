namespace Speak.Telegram.MusicQuizFeatureContracts;

public class SendMusicQuizAnswerFeatureRequest
{
    public SendMusicQuizAnswerFeatureRequest(long? chatId, string callbackId, string? playerUsername, 
        int? messageIdWithButtonPushed, AnswerCallback answer)
    {
        ChatId = chatId;
        CallbackId = callbackId;
        PlayerUsername = playerUsername;
        MessageIdWithButtonPushed = messageIdWithButtonPushed;
        Answer = answer;
    }

    /// <summary>
    /// Идентификатор чата, в котором нажали кнопку на сообщении с музыкой
    /// </summary>
    public long? ChatId { get; }
    
    /// <summary>
    /// Идентификатор сообщения с музыкой
    /// </summary>
    public int? MessageIdWithButtonPushed { get; }

    /// <summary>
    /// Идентификатор Callback, на который надо ответить
    /// </summary>
    public string CallbackId { get; }

    /// <summary>
    /// Кто нажал кнопку
    /// </summary>
    public string? PlayerUsername { get; }

    /// <summary>
    /// Какой пришел ответ
    /// </summary>
    public AnswerCallback Answer { get; }
}