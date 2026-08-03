using Pulonia.Domain.Entities;

namespace Pulonia.Application.Common.Interfaces.Repositories;

public interface IDailySlaRepository
{
    Task UpsertDailySlaAsync(IEnumerable<DailySla> slaUpdates, CancellationToken cancellationToken);
}