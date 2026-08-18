using Pulonia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulonia.Infrastructure.Data.Configurations;

public class DailySlaConfiguration : IEntityTypeConfiguration<DailySla>
{
    public void Configure(EntityTypeBuilder<DailySla> builder)
    {
        builder.HasKey(x => new { x.StoreId, x.Date });

        builder.Property(x => x.TotalPings).IsRequired();
        builder.Property(x => x.SuccessPings).IsRequired();

        builder.Property(x => x.MinimumLatency).IsRequired();
        builder.Property(x => x.MaximumLatency).IsRequired();
        builder.Property(x => x.TotalLatency).IsRequired();

        // Analytics
        builder.Property(x => x.AvgLatency).HasPrecision(5, 2);
        builder.Property(x => x.SlaPercentage).HasPrecision(5, 2);

        // Relation
        builder.HasOne(x => x.Store)
            .WithMany()
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}