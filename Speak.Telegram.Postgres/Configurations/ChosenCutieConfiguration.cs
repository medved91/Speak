using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speak.Telegram.CutieFeature.Contracts;

namespace Speak.Telegram.Postgres.Configurations;

public class ChosenCutieConfiguration : IEntityTypeConfiguration<ChosenCutie>
{
    public void Configure(EntityTypeBuilder<ChosenCutie> builder)
    {
        builder.ToTable("ChosenCuties");
        
        builder.Property<string>("PlayerUsername");

        builder.Property<int>("ChatsTableId");
        
        builder.HasKey("ChatsTableId", "PlayerUsername", "WhenChosen");

        builder.HasOne(c => c.Player)
            .WithMany()
            .IsRequired()
            .HasForeignKey("ChatsTableId", "PlayerUsername");

        builder.HasOne(c => c.Mission)
            .WithMany()
            .IsRequired();

        builder.Property(b => b.WhenChosen)
            .IsRequired();
    }
}