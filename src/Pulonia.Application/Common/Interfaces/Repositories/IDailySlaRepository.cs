using Pulonia.Application.Common.Models;
using Pulonia.Domain.Entities;

namespace Pulonia.Application.Common.Interfaces.Repositories;

public interface IDailySlaRepository
{
    Task UpsertDailySlaAsync(IEnumerable<DailySla> slaUpdates, CancellationToken cancellationToken);
    Task<PagedList<DailySla>> GetSlaByDateAsync(PaginationParams paginationParams, DateOnly date, CancellationToken cancellationToken, bool trackChanges = false);
}