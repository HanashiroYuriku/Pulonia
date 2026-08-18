using Pulonia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulonia.Infrastructure.Data.Configurations;

public class StorePingLogConfiguration : IEntityTypeConfiguration<StorePingLog>
{
    // Database table scheme for StorePingLog
    public void Configure(EntityTypeBuilder<StorePingLog> builder)
    {
        // Set Composite Key
        builder.HasKey(x => new { x.StoreId, x.Ip, x.CheckedAt });

        builder.Property(x => x.StoreId).IsRequired();
        builder.Property(x => x.IpType).IsRequired();
        builder.Property(x => x.Ip).IsRequired().HasMaxLength(15);
        builder.Property(x => x.PingStatus).IsRequired();
        builder.Property(x => x.Latency).IsRequired();
    }
}