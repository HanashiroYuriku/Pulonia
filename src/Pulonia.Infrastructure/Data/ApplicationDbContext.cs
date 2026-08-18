using Pulonia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Pulonia.Application.Common.Interfaces.Authentication;

namespace Pulonia.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
    }

    // Table
    public DbSet<User> Users => Set<User>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<StorePingLog> StorePingLogs => Set<StorePingLog>();
    public DbSet<DailySla> DailySlas => Set<DailySla>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<MasterProvider> MasterProviders => Set<MasterProvider>();
    public DbSet<StoreState> StoreStates => Set<StoreState>();

    // .. add other table here

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // scan and implement all file of IEntityTypeConfiguration
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        var branchId = _currentUserService.BranchId;
        var role = _currentUserService.Role;
        bool isSuperAdmin = role == "HeadOffice";

        modelBuilder.Entity<Store>()
            .HasQueryFilter(s => isSuperAdmin || s.BranchId == branchId);

        modelBuilder.Entity<User>()
        .HasQueryFilter(u => isSuperAdmin || u.BranchId == branchId);

        modelBuilder.Entity<DailySla>()
            .HasQueryFilter(sla => isSuperAdmin || sla.Store!.BranchId == branchId);

        modelBuilder.Entity<Incident>()
            .HasQueryFilter(inc => isSuperAdmin || inc.Store!.BranchId == branchId);

        modelBuilder.Entity<StoreState>()
            .HasQueryFilter(state => isSuperAdmin || state.Store!.BranchId == branchId);

        // Hati-hati: Filter log ini mungkin memperberat query jika tidak di-index dengan baik,
        // pastikan Worker (yang tidak punya _currentUserService) memanggil .IgnoreQueryFilters()
        modelBuilder.Entity<StorePingLog>()
            .HasQueryFilter(log => isSuperAdmin || log.Store!.BranchId == branchId);
    }
}