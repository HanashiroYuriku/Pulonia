using LinqToDB;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pulonia.Application.Common.Interfaces.Repositories;
using Pulonia.Application.Common.Models;
using Pulonia.Domain.Entities;
using Pulonia.Infrastructure.Data.Extensions;

namespace Pulonia.Infrastructure.Data.Repositories;

public class DailySlaRepository : IDailySlaRepository
{
    private readonly ApplicationDbContext _ctx;

    public DailySlaRepository(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<PagedList<DailySla>> GetSlaByDateAsync(PaginationParams paginationParams, DateOnly date, CancellationToken cancellationToken, bool trackChanges = false)
    {
        var query = _ctx.Set<DailySla>().AsQueryable();

        if (!trackChanges) query = query.AsNoTracking();
        query = query.Where(sla => sla.Date == date)
            .OrderBy(sla => sla.StoreCode);
        
        return await query.ToPagedListAsync(paginationParams.PageNumber, paginationParams.PageSize, cancellationToken);
    }

    public async Task UpsertDailySlaAsync(IEnumerable<DailySla> slaUpdates, CancellationToken cancellationToken)
    {
        await _ctx.GetTable<DailySla>()
            .Merge()
            .Using(slaUpdates)
            .On((target, source) => target.StoreCode == source.StoreCode && target.Date == source.Date)
            .InsertWhenNotMatched(source => new DailySla
            {
                StoreCode = source.StoreCode,
                Date = source.Date,
                TotalPings = source.TotalPings,
                SuccessPings = source.SuccessPings,
            })
            .UpdateWhenMatched((target, source) => new DailySla
            {
                TotalPings = target.TotalPings + source.TotalPings,
                SuccessPings = target.SuccessPings + source.SuccessPings,
            })
            .MergeAsync(cancellationToken);
    }
}