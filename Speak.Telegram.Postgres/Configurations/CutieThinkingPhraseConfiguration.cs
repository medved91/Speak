using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speak.Telegram.CutieFeature.Contracts;

namespace Speak.Telegram.Postgres.Configurations;

public class CutieThinkingPhraseConfiguration : IEntityTypeConfiguration<CutieThinkingPhrase>
{
    public void Configure(EntityTypeBuilder<CutieThinkingPhrase> builder)
    {
        builder.ToTable("CutieThinkingPhrases");

        builder.HasNoKey();

        builder.Property(p => p.Phrase).IsRequired();
    }
}