namespace Pulonia.Domain.Entities;

public class DailySla
{
    // index
    public string StoreCode { get; set; } = string.Empty;
    // index
    public DateOnly Date { get; set; }
    public int TotalPings { get; set; }
    public int SuccessPings { get; set; }
    public int MinimumLatency { get; set; }
    public int MaximumLatency { get; set; }
    public long TotalLatency { get; set; }

    public decimal AvgLatency => SuccessPings > 0 
        ? TotalLatency / SuccessPings
        : 0;

    public decimal SlaPercentage => TotalPings > 0 
        ? Math.Round((decimal)SuccessPings / TotalPings * 100, 2) 
        : 0;
        
    public Store? Store { get; set; }
}