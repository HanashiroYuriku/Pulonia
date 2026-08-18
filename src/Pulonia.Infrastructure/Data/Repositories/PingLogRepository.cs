using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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

    public async Task RotatePartitionsASync(int retentionDays, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;

        // count partition name to delete
        var dateToDrop = today.AddDays(-retentionDays);
        string partitionToDrop = $"p_{dateToDrop:yyyyMMdd}";
        string dropSql = $"ALTER TABLE StorePingLogs DROP PARTITION {partitionToDrop};";

        try
        {
            await _ctx.Database.ExecuteSqlRawAsync(dropSql, cancellationToken);
        }
        catch
        {

        }

        // preparing next partition
        var dateToCreate = today.AddDays(retentionDays);
        string newPartitionName = $"p_{dateToCreate:yyyyMMdd}";
        string nextDayLimit = dateToCreate.AddDays(1).ToString("yyyy-MM-dd");

        // preparing query
        string reorganizeSql = $@"
            ALTER TABLE StorePingLogs
            REORGANIZE PARTITION p_future INTO (
                PARTITION {newPartitionName} VALUES LESS THAN (TO_DAYS('{nextDayLimit}')),
                PARTITION p_future VALUES LESS THAN MAXVALUE
            );
        ";

        // excecution drop partition
        try
        {
            await _ctx.Database.ExecuteSqlRawAsync(reorganizeSql, cancellationToken);
        }
        catch (Exception)
        {

        }
    }
}