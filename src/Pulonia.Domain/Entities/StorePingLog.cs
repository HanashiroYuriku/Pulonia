using Pulonia.Domain.Enums;

namespace Pulonia.Domain.Entities;

public class StorePingLog
{
    public string Ip { get; set; } = string.Empty;
    public string StoreCode { get; set; } = string.Empty;
    public IpType IpType { get; set; }
    public bool IsSuccess { get; set; } = false;
    public int Latency { get; set; }
    public DateTime CheckedAt { get; set; }
}