using Pulonia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulonia.Infrastructure.Data.Configurations;

public class DailySlaConfiguration : IEntityTypeConfiguration<DailySla>
{
    // Database table scheme for DailySla
    public void Configure(EntityTypeBuilder<DailySla> builder)
    {
        // Set Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.StoreCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.TotalPings).IsRequired();
        builder.Property(x => x.SuccessPings).IsRequired();
        builder.Property(x => x.SlaPercentage);

        builder.HasOne(x => x.Store)
            .WithMany()
            .HasForeignKey(x => x.StoreCode)
            .OnDelete(DeleteBehavior.Restrict);
    }
}