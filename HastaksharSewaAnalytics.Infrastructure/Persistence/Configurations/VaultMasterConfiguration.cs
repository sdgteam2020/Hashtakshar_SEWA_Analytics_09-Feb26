using HastaksharSewaAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaksharSewaAnalytics.Infrastructure.Persistence.Configurations;

public sealed class VaultMasterConfiguration : IEntityTypeConfiguration<VaultMaster>
{
    public void Configure(EntityTypeBuilder<VaultMaster> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.SerialNo).IsRequired();
        builder.Property(x => x.Public_Key).IsRequired();
        builder.Property(x => x.ValidFrom).HasColumnType("timestamp with time zone");
        builder.Property(x => x.ValidTo).HasColumnType("timestamp with time zone");
        builder.HasIndex(x => x.SerialNo).IsUnique();
    }
}
