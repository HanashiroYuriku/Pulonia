using Pulonia.Domain.Entities;

namespace Pulonia.Application.Common.Interfaces.Repositories;

public interface IStoreRepository
{
    Task<IEnumerable<Store>> GetAllActiveStoresAsync(CancellationToken cancellationToken, bool trackChanges = false);
}