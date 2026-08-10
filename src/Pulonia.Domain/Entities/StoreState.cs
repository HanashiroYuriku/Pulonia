using Pulonia.Domain.Enums;

namespace Pulonia.Domain.Entities;

public class StoreState
{
    public string StoreCode { get; set; } = string.Empty;
    public int ConsecutiveFails { get; set; }
    public bool IsAlertSent { get; set; } = false;
    public DateTime LastPingTime { get; set; }
    public int? ActiveIncidentId { get; set; }

    public StoreStatus CurrentStatus
    {
        get
        {
            if (ActiveIncidentId.HasValue) return StoreStatus.Offline;

            if (ConsecutiveFails > 0) return StoreStatus.Warning;

            return StoreStatus.Online;
        }
    }
}