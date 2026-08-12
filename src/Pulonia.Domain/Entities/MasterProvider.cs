using Pulonia.Domain.Common;
using Pulonia.Domain.Enums;

namespace Pulonia.Domain.Entities;

public class MasterProvider : BaseEntity<int>
{
    public string ProviderName { get; set; } = string.Empty;
    public ConnectionType NetworkCategory { get; set; } // Fiber Optic or VSat
    public bool EnableMainAlert { get; set; } = true;
    public bool EnableBackupAlert { get; set; } = false;
}