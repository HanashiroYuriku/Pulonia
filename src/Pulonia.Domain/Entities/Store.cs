using Pulonia.Domain.Enums;

namespace Pulonia.Domain.Entities;

public class Store
{
    // will set as PK
    public string StoreCode { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
    // will set as PK
    public string IpAddress  { get; set; } = string.Empty;
    public string? IpRouter { get; set; } = string.Empty;
    public ConnectionType ConnectionType { get; set; } =  ConnectionType.Fiber;
    public bool IsActive { get; set; } = true;
}