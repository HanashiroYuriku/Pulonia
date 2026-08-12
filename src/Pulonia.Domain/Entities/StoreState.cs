using Pulonia.Domain.Enums;

namespace Pulonia.Domain.Entities;

public class StoreState
{
    public string StoreCode { get; set; } = string.Empty;
    public StoreStatus CurrentStatus { get; set; } = StoreStatus.Down;
    public int ConsecutiveFails { get; set; } = 0;
    public bool IsAlertSent { get; set; } = false;
    public DateTime LastPingTime { get; set; }
    public int? ActiveIncidentId { get; set; }
}