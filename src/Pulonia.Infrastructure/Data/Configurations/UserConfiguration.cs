using Pulonia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulonia.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    // Database table scheme for user
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Set Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.NIK).IsRequired().HasMaxLength(20);
        builder.Property(x => x.FullName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(150);
        builder.Property(x => x.PasswordHash).IsRequired();
        builder.Property(x => x.Role).IsRequired();

        // unique and index data
        builder.HasIndex(x => x.NIK).IsUnique();
        builder.HasIndex(x => x.RefreshToken).IsUnique();
    }
}