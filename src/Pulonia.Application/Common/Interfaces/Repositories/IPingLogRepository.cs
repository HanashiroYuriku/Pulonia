using Pulonia.Domain.Entities;

namespace Pulonia.Application.Common.Interfaces.Repositories;

public interface IPingLogRepository
{
    Task BulkInsertAsync(IEnumerable<StorePingLog> logs, CancellationToken cancellationToken);
    Task RotatePartitionsASync(int retentionDays, CancellationToken cancellationToken);
}