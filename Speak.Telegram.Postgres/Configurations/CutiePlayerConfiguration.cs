using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speak.Telegram.CutieFeature.Contracts;

namespace Speak.Telegram.Postgres.Configurations;

public class CutiePlayerConfiguration : IEntityTypeConfiguration<CutiePlayer>
{
    public void Configure(EntityTypeBuilder<CutiePlayer> builder)
    {
        builder.ToTable("CutiePlayers");

        builder.Property<int>("ChatsTableId");

        builder.HasKey("ChatsTableId", "TelegramUsername");

        builder
            .HasOne(p => p.Chat)
            .WithMany()
            .HasForeignKey("ChatsTableId");
    }
}