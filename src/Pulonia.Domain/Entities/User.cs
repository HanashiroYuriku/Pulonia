using Pulonia.Domain.Common;
using Pulonia.Domain.Enums;

namespace Pulonia.Domain.Entities;

public class User : BaseEntity
{
    public string NIK { get; private set; } = null!;
    public string FullName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public Role Role { get; private set; }

    // Auth
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }

    public int? BranchId { get; private set; }

    public User()
    {

    }
    // Constructor for create new User
    public User(string nik, string fullName, string email, string passwordHash, Role role, int? branchId, string createdBy)
    {
        NIK = nik;
        FullName = fullName;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        BranchId = branchId;
        CreatedBy = createdBy;
    }

    // Update full name and email user
    public void UpdateProfile(string fullName, string email)
    {
        FullName = fullName;
        Email = email;
    }

    // Update password user
    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }

    // Update refresh token and expiry 
    public void UpdateRefreshToken(string token, DateTime expTime)
    {
        RefreshToken = token;
        RefreshTokenExpiryTime = expTime;
    }

    // Delete refresh token user (logout case)
    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
    }
}