using Pulonia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulonia.Infrastructure.Data.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    // Database table scheme for Store
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        // Set Primary Key
        builder.HasKey(x => x.StoreCode);

        builder.Property(x => x.StoreName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.IpAddress).IsRequired().HasMaxLength(15);
        builder.Property(x => x.IpRouter).IsRequired().HasMaxLength(15);
        builder.Property(x => x.ConnectionType).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
    }
}