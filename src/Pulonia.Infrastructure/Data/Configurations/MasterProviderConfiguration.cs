using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pulonia.Domain.Entities;

namespace Pulonia.Infrastructure.Data.Configurations;

public class MasterProviderConfiguration : IEntityTypeConfiguration<MasterProvider>
{
    public void Configure(EntityTypeBuilder<MasterProvider> builder)
    {
        // == Base Entity ==
        // Set Primary Key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        builder.Property(x => x.ProviderName)
            .IsRequired()
            .HasMaxLength(100);
    }
}