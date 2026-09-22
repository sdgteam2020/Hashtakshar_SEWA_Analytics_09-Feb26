using HastaksharSewaAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaksharSewaAnalytics.Infrastructure.Persistence.Configurations;

public sealed class HastaksharSewaInstallationConfiguration : IEntityTypeConfiguration<HastaksharSewaInstallation>
{
    public void Configure(EntityTypeBuilder<HastaksharSewaInstallation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.CreatedByClientId, x.VersionId }).IsUnique();
        builder.HasOne<ClientMaster>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByClientId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ApplicationVersion>()
            .WithMany()
            .HasForeignKey(x => x.VersionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
