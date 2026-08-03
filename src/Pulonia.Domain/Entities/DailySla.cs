namespace Pulonia.Domain.Entities;

public class DailySla
{
    public int Id { get; set; }
    public string StoreCode { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public int TotalPings { get; set; }
    public int SuccessPings { get; set; }
    public decimal SlaPercentage { get; set; }

    public Store? Store { get; set; }
}