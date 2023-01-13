using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speak.Telegram.CommonContracts;

namespace Speak.Telegram.Postgres.Configurations;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.ToTable("Chats");

        builder.Property<int>("Id");
        builder.HasKey("Id");
        builder.Property(c => c.TelegramChatId).IsRequired();

        builder.HasIndex(c => c.TelegramChatId).IsUnique();
    }
}