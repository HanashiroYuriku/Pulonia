using Pulonia.Domain.Common;
using Pulonia.Domain.Enums;

namespace Pulonia.Domain.Entities;

public class Store : BaseEntity<int>
{
    public string StoreCode { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string StoreAddress { get; set; } = string.Empty;

    // Main Ip
    public string MainIpAddress { get; set; } = string.Empty;
    public string MainIpRouter { get; set; } = string.Empty;
    public int MainProviderId { get; set; }

    // Backup Ip
    public string BackupIpAddress { get; set; } = string.Empty;
    public string BackupIpRouter { get; set; } = string.Empty;
    public int BackupProviderId { get; set; }

    public bool IsActive { get; set; } = true;
}