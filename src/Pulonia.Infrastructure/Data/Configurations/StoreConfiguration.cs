using Pulonia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulonia.Infrastructure.Data.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    // Database table scheme for Store
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        // == Base Entity ==
        // Set Primary Key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        // Make Store Code unique and have index
        builder.Property(x => x.StoreCode);
        builder.HasIndex(x => x.StoreCode).IsUnique();

        builder.Property(x => x.StoreName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.StoreAddress).IsRequired().HasMaxLength(200);
        // == Main Connection
        // Ip Address
        builder.Property(x => x.MainIpAddress).IsRequired().HasMaxLength(15);
        // Ip Router
        builder.Property(x => x.MainIpRouter).IsRequired().HasMaxLength(15);
        // Provider
        builder.Property(x => x.MainProviderId).IsRequired();

        // == Backup Connection
        // Ip Address
        builder.Property(x => x.BackupIpAddress).IsRequired().HasMaxLength(15);
        // Ip Router
        builder.Property(x => x.BackupIpRouter).IsRequired().HasMaxLength(15);
        // Provider
        builder.Property(x => x.BackupProviderId).IsRequired();

        builder.Property(x => x.IsActive).IsRequired();

        builder.HasOne(x => x.Branch)
            .WithMany()
            .HasForeignKey(x => x.BranchCode)
            .OnDelete(DeleteBehavior.Restrict);
    }
}