namespace Pulonia.Application.Common.Configurations;

public class PingConfiguration
{
    public int MaxRetries { get; set; } = 3;
    public int TimeoutMs { get; set; } = 5000;
    public int MaxDegreeOfParallelism { get; set; } = 500;
}