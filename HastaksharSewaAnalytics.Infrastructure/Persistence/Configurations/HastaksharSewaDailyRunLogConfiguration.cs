using HastaksharSewaAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaksharSewaAnalytics.Infrastructure.Persistence.Configurations;

public sealed class HastaksharSewaDailyRunLogConfiguration : IEntityTypeConfiguration<HastaksharSewaDailyRunLog>
{
    public void Configure(EntityTypeBuilder<HastaksharSewaDailyRunLog> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.CreatedByClientId, x.CreatedAt }).IsUnique();
        builder.HasOne<HastaksharSewaInstallation>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
