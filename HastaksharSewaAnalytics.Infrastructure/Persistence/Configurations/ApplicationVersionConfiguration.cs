using HastaksharSewaAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaksharSewaAnalytics.Infrastructure.Persistence.Configurations;

public sealed class ApplicationVersionConfiguration : IEntityTypeConfiguration<ApplicationVersion>
{
    public void Configure(EntityTypeBuilder<ApplicationVersion> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Version).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.ApplicationId, x.Version }).IsUnique();
        builder.HasOne<ApplicationMaster>()
            .WithMany()
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
