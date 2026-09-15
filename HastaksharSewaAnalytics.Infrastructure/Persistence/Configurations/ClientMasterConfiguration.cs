using HastaksharSewaAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaksharSewaAnalytics.Infrastructure.Persistence.Configurations;

public sealed class ClientMasterConfiguration : IEntityTypeConfiguration<ClientMaster>
{
    public void Configure(EntityTypeBuilder<ClientMaster> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.DomainId).HasMaxLength(64).IsRequired();
        builder.Property(x => x.IPAddress).HasMaxLength(64).IsRequired();
        builder.HasIndex(x => x.DomainId).IsUnique();
        builder.HasOne<Device>()
            .WithMany()
            .HasForeignKey(x => x.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
