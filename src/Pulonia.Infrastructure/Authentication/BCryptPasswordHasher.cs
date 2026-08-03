
using Pulonia.Application.Common.Interfaces.Authentication;

namespace Pulonia.Infrastructure.Authentication;

public class BycryptPasswordHasher : IPasswordHasher
{
    // Hasing password
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // Compare hashing password and password input
    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}