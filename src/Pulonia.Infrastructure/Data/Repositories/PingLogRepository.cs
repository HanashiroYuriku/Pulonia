using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using Pulonia.Application.Common.Interfaces.Repositories;
using Pulonia.Domain.Entities;

namespace Pulonia.Infrastructure.Data.Repositories;

public class PingLogRepository : IPingLogRepository
{
    private readonly ApplicationDbContext _ctx;

    public PingLogRepository(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task BulkInsertAsync(IEnumerable<StorePingLog> logs, CancellationToken cancellationToken)
    {
        var opt = new BulkCopyOptions
        {
            BulkCopyType = BulkCopyType.MultipleRows
        };

        await _ctx.BulkCopyAsync(opt, logs, cancellationToken);
    }
}