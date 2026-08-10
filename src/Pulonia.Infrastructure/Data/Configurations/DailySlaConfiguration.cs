using Pulonia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulonia.Infrastructure.Data.Configurations;

public class DailySlaConfiguration : IEntityTypeConfiguration<DailySla>
{
    public void Configure(EntityTypeBuilder<DailySla> builder)
    {
        builder.Property(x => x.StoreCode).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.TotalPings).IsRequired();
        builder.Property(x => x.SuccessPings).IsRequired();
        
        builder.Property(x => x.SlaPercentage).HasColumnType("decimal(5,2)"); 

        builder.HasIndex(x => new { x.StoreCode, x.Date }).IsUnique();
        
        // Relation
        builder.HasOne(x => x.Store)
            .WithMany()
            .HasForeignKey(x => x.StoreCode)
            .OnDelete(DeleteBehavior.Restrict);
    }
}