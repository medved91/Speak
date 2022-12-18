namespace Speak.Telegram.CutieFeature.Contracts.Requests;

public class SendMissionResultFeatureRequest
{
    public long ChatId { get; }
    public int RepliedToBotMessageId { get; }
    public string UsernameWhoSentPhoto { get; }
    public int UserReplyMessageId { get; }
}