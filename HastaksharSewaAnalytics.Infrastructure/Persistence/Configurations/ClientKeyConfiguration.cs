using HastaksharSewaAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaksharSewaAnalytics.Infrastructure.Persistence.Configurations;

public sealed class ClientKeyConfiguration : IEntityTypeConfiguration<ClientKey>
{
    public void Configure(EntityTypeBuilder<ClientKey> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => x.CreatedByClientId).IsUnique();
        builder.HasOne<ClientMaster>()
            .WithOne()
            .HasForeignKey<ClientKey>(x => x.CreatedByClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
