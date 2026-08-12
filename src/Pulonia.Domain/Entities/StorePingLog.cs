using Pulonia.Domain.Enums;

namespace Pulonia.Domain.Entities;

public class StorePingLog
{
    public string StoreCode { get; set; } = string.Empty;
    public DateTime CheckedAt { get; set; }
    public string Ip { get; set; } = string.Empty;
    public IpType IpType { get; set; }
    public PingStatus MyProperty { get; set; }
    public int Latency { get; set; }
}