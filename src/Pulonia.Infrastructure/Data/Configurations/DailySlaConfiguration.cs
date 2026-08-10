using Pulonia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulonia.Infrastructure.Data.Configurations;

public class DailySlaConfiguration : IEntityTypeConfiguration<DailySla>
{
    public void Configure(EntityTypeBuilder<DailySla> builder)
    {
        builder.HasKey(x => new { x.StoreCode, x.Date });

        builder.Property(x => x.TotalPings).IsRequired();
        builder.Property(x => x.SuccessPings).IsRequired();
        builder.Property(x => x.MinimumLatency).IsRequired();
        builder.Property(x => x.MaximumLatency).IsRequired();
        builder.Property(x => x.TotalLatency).IsRequired();
        
        // Relation
        builder.HasOne(x => x.Store)
            .WithMany()
            .HasForeignKey(x => x.StoreCode)
            .OnDelete(DeleteBehavior.Restrict);

        // ignore database schema
        builder.Ignore(x => x.AvgLatency);
        builder.Ignore(x => x.SlaPercentage);
    }
}