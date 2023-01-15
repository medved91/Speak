using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speak.Telegram.MusicQuizFeatureContracts;

namespace Speak.Telegram.Postgres.Configurations;

public class MusicQuizRoundConfiguration : IEntityTypeConfiguration<MusicQuizRound>
{
    public void Configure(EntityTypeBuilder<MusicQuizRound> builder)
    {
        builder.ToTable("MusicQuizRounds");
        
        builder.Property<int>("ChatsTableId");
        
        builder.HasKey("ChatsTableId", "PlayerUsername", "RoundMessageId");

        builder.HasOne(c => c.Chat)
            .WithMany()
            .IsRequired()
            .HasForeignKey("ChatsTableId");

        builder.Property(q => q.Artist).IsRequired();
        builder.Property(q => q.Title).IsRequired();
    }
}