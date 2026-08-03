using Pulonia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Pulonia.Application.Common.Interfaces.Repositories;

namespace Pulonia.Infrastructure.Data.Repositories;


public class StoreRepository : IStoreRepository
{
    private readonly ApplicationDbContext _ctx;

    public StoreRepository(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<IEnumerable<Store>> GetAllActiveStoresAsync(CancellationToken cancellationToken, bool trackChanges = false)
    {
        var query = _ctx.Stores.AsQueryable();

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query.Where(s => s.IsActive).ToListAsync(cancellationToken);
    }
}
