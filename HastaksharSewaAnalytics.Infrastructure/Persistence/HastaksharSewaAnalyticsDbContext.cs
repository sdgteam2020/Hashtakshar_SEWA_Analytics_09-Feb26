using HastaksharSewaAnalytics.Domain.Entities;
using HastaksharSewaAnalytics.Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HastaksharSewaAnalytics.Infrastructure.Persistence;

public sealed class HastaksharSewaAnalyticsDbContext
    : IdentityDbContext<ApplicationUser>
{
    public HastaksharSewaAnalyticsDbContext(DbContextOptions<HastaksharSewaAnalyticsDbContext> options)
        : base(options)
    {
    }

    public DbSet<VaultMaster> VaultMasters => Set<VaultMaster>();
    public DbSet<HastaksharSewaDailyRunLog> HastaksharSewaDailyRunLogs => Set<HastaksharSewaDailyRunLog>();
    public DbSet<HastaksharSewaInstallation> HastaksharSewaInstallations => Set<HastaksharSewaInstallation>();
    public DbSet<ClientErrorLog> ClientErrorLogs => Set<ClientErrorLog>();
    public DbSet<DigitalSignDetail> DigitalSignDetails => Set<DigitalSignDetail>();
    public DbSet<Device> Devices => Set<Device>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<DigitalSignDetail>()
            .HasOne<VaultMaster>()
            .WithMany()
            .HasForeignKey(d => d.ValtMasterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
