using Pulonia.Domain.Entities;

namespace Pulonia.Application.Common.Interfaces.Repositories;

// User Repository Interface / Contract
public interface IUserRepository
{
    // Get User by ID
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool trackChanges = false);

    // Get User by Email
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken, bool trackChanges = false);

    // Get User by Refresh 
    Task<User?> GetUserByRefreshToken(string refreshToken, CancellationToken cancellationToken, bool trackChanges = false);

    // Get All Users
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken, bool trackChanges = false); // will update using pagination

    // Add new User
    void Add(User user);

    // Update a spesific User
    void Update(User user);

    // Delete a User
    void Delete(User user);
}