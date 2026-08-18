using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulonia.Domain.Entities;

namespace Pulonia.Infrastructure.Data.Configurations;

public class StoreStateConfiguration : IEntityTypeConfiguration<StoreState>
{
    public void Configure(EntityTypeBuilder<StoreState> builder)
    {
        builder.HasKey(x => x.StoreId);

        builder.Property(x => x.ConsecutiveFails).IsRequired();

        builder.HasOne(x => x.Store)
            .WithOne()
            .HasForeignKey<StoreState>(x => x.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Incident>()
       .WithMany()
       .HasForeignKey(x => x.ActiveIncidentId)
       .IsRequired(false)
       .OnDelete(DeleteBehavior.SetNull);
    }
}