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
        builder.Property(x => x.RunOnDate).HasColumnType("date").IsRequired();
        builder.HasIndex(x => new { x.ClientId, x.RunOnDate }).IsUnique();
        builder.HasOne<ClientMaster>()
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ApplicationVersion>()
            .WithMany()
            .HasForeignKey(x => x.VersionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
