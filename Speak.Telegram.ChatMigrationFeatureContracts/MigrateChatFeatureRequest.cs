namespace Speak.Telegram.ChatMigrationFeatureContracts;

public class MigrateChatFeatureRequest
{
    public MigrateChatFeatureRequest(long oldChatId, long? newChatId)
    {
        OldChatId = oldChatId;
        NewChatId = newChatId;
    }

    public long OldChatId { get; }
    
    public long? NewChatId { get; }
}