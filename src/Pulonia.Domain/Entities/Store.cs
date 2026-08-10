using Pulonia.Domain.Enums;

namespace Pulonia.Domain.Entities;

public class Store
{
    public string StoreCode { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string StoreAddress { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string? IpRouter { get; set; }
    public ConnectionType ConnectionType { get; set; } = ConnectionType.Fiber;
    public bool IsActive { get; set; } = true;

    public Branch? Branch { get; set; }
}