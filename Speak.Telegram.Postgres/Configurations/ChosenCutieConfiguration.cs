using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speak.Telegram.CutieFeature.Contracts;

namespace Speak.Telegram.Postgres.Configurations;

public class ChosenCutieConfiguration : IEntityTypeConfiguration<ChosenCutie>
{
    public void Configure(EntityTypeBuilder<ChosenCutie> builder)
    {
        builder.ToTable("ChosenCuties");

        builder.HasKey(c => new { c.Player.ChatId, c.Player.TelegramUsername });
        
        builder.HasOne(c => c.Player)
            .WithMany()
            .IsRequired();

        builder.HasOne(c => c.Mission)
            .WithMany()
            .IsRequired();

        builder.Property(b => b.WhenChosen)
            .IsRequired()
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
    }
}