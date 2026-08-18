using Pulonia.Domain.Enums;

namespace Pulonia.Domain.Entities;

public class StoreState
{
    public int StoreId { get; set; }
    public StoreStatus CurrentStatus { get; set; } = StoreStatus.Down;
    public int ConsecutiveFails { get; set; } = 0;
    public bool IsAlertSent { get; set; } = false;
    public DateTime LastPingTime { get; set; }
    public int? ActiveIncidentId { get; set; }

    public StoreState()
    {

    }

    public Store? Store { get; set; }
}