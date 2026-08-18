namespace Pulonia.Application.Common.Interfaces.Authentication;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Role { get; }
    int? BranchId { get; }
}