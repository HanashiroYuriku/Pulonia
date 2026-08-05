using Pulonia.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Pulonia.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    // Table
    public DbSet<User> Users => Set<User>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<StorePingLog> StorePingLogs => Set<StorePingLog>();
    public DbSet<DailySla> DailySlas => Set<DailySla>();

    // .. add other table here

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // scan and implement all file of IEntityTypeConfiguration
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}