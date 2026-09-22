using HastaksharSewaAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaksharSewaAnalytics.Infrastructure.Persistence.Configurations;

public sealed class ApplicationMasterConfiguration : IEntityTypeConfiguration<ApplicationMaster>
{
    public void Configure(EntityTypeBuilder<ApplicationMaster> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.AppName).IsRequired();
    }
}
