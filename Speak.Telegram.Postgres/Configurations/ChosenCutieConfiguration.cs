using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speak.Telegram.CutieFeature.Contracts;

namespace Speak.Telegram.Postgres.Configurations;

public class ChosenCutieConfiguration : IEntityTypeConfiguration<ChosenCutie>
{
    public void Configure(EntityTypeBuilder<ChosenCutie> builder)
    {
        builder.ToTable("ChosenCuties");

        builder.Property<long>("ChatId");
        builder.Property<string>("PlayerUsername");

        builder.HasKey("ChatId", "PlayerUsername");
        
        builder.HasOne(c => c.Player)
            .WithMany()
            .IsRequired()
            .HasForeignKey("ChatId", "PlayerUsername");

        builder.HasOne(c => c.Mission)
            .WithMany()
            .IsRequired();

        builder.Property(b => b.WhenChosen)
            .IsRequired();
    }
}