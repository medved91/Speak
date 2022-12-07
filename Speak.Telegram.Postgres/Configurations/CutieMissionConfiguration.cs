using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speak.Telegram.CutieFeature.Contracts;

namespace Speak.Telegram.Postgres.Configurations;

public class CutieMissionConfiguration : IEntityTypeConfiguration<CutieMission>
{
    public void Configure(EntityTypeBuilder<CutieMission> builder)
    {
        builder.ToTable("CutieMissions");

        builder.Property<int>("Id").ValueGeneratedOnAdd();
        builder.HasKey("Id");

        builder.Property(m => m.Description).IsRequired();
    }
}