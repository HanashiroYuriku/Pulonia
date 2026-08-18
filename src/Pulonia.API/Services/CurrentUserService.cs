using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Pulonia.Application.Common.Interfaces.Authentication;

namespace Pulonia.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || user.Identity?.IsAuthenticated != true) return null;

            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        }
    }

    public string? Role => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

    public int? BranchId
    {
        get
        {
            var branchClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("branchId")?.Value;

            return int.TryParse(branchClaim, out var branchId) ? branchId : null;
        }
    }
}