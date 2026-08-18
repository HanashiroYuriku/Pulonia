using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulonia.Domain.Entities;

namespace Pulonia.Infrastructure.Data.Configurations;

public class IncidentConfiguration : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.StoreId).IsRequired();

        builder.HasOne<Store>()
        .WithMany()
        .HasForeignKey(x => x.StoreId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}