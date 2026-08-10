using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulonia.Domain.Entities;

namespace Pulonia.Infrastructure.Data.Configurations;

public class IncidentConfiguration : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Store)
        .WithMany()
        .HasForeignKey(x => x.StoreCode)
        .OnDelete(DeleteBehavior.Restrict);
    }
}