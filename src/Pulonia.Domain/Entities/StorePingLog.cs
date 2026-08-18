using Pulonia.Domain.Enums;

namespace Pulonia.Domain.Entities;

public class StorePingLog
{
    public int StoreId { get; set; }
    public DateTime CheckedAt { get; set; }
    public string Ip { get; set; } = string.Empty;
    public IpType IpType { get; set; }
    public PingStatus PingStatus { get; set; }
    public int Latency { get; set; }

    public StorePingLog()
    {

    }

    public Store? Store { get; set; }
}