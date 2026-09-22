using HastaksharSewaAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HastaksharSewaAnalytics.Infrastructure.Persistence.Configurations;

public sealed class ClientErrorLogConfiguration : IEntityTypeConfiguration<ClientErrorLog>
{
    public void Configure(EntityTypeBuilder<ClientErrorLog> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.ErrorMessage).IsRequired();     
        builder.HasOne<ClientMaster>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByClientId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ApplicationVersion>()
            .WithMany()
            .HasForeignKey(x => x.ApplicationVersionId)
            .OnDelete(DeleteBehavior.Restrict);        
    }
}
