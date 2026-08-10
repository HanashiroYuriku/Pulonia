using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulonia.Domain.Entities;

namespace Pulonia.Infrastructure.Data.Configurations;

public class StoreStateConfiguration : IEntityTypeConfiguration<StoreState>
{
    public void Configure(EntityTypeBuilder<StoreState> builder)
    {
        builder.HasKey(x => x.StoreCode);
        builder.Property(x => x.ConsecutiveFails).IsRequired();

        builder.Ignore(x => x.CurrentStatus);
        builder.HasOne<Store>()
            .WithOne()
            .HasForeignKey<StoreState>(x => x.StoreCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Incident>()
       .WithMany()
       .HasForeignKey(x => x.ActiveIncidentId)
       .IsRequired(false)
       .OnDelete(DeleteBehavior.SetNull);
    }
}