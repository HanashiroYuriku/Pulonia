using Pulonia.Application.Common.Interfaces.Authentication;

namespace Pulonia.Worker.Services;

public class SystemUserService : ICurrentUserService
{
    public string? UserId => "System-Worker";

    public string? Role => "HeadOffice";

    public int? BranchId => null;
}