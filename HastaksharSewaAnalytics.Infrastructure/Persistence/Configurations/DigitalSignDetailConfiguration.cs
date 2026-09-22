using HastaksharSewaAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaksharSewaAnalytics.Infrastructure.Persistence.Configurations;

public sealed class DigitalSignDetailConfiguration : IEntityTypeConfiguration<DigitalSignDetail>
{
    public void Configure(EntityTypeBuilder<DigitalSignDetail> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.SignDateTime).HasColumnType("timestamp with time zone");
        builder.HasOne<VaultMaster>()
            .WithMany()
            .HasForeignKey(x => x.VaultMasterId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ClientMaster>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
