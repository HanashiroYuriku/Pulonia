namespace Pulonia.Domain.Entities;

public class DailySla
{
    // index
    public int StoreId { get; set; }
    // index
    public DateOnly Date { get; set; }
    public int TotalPings { get; set; }
    public int SuccessPings { get; set; }
    public int MinimumLatency { get; set; }
    public int MaximumLatency { get; set; }
    public long TotalLatency { get; set; }

    public decimal AvgLatency { get; set; }
    public decimal SlaPercentage { get; set; }

    public int DownFrequency { get; set; }
    public int TotalDowntimeSeconds { get; set; }

    public Store? Store { get; set; }

    public DailySla()
    {

    }
}