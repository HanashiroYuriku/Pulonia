namespace Pulonia.Application.Common.Interfaces.ExternalServices;

public interface INetworkService
{
    Task<(bool IsSuccess, int Latency)> PingWithRetryAsync(string ipAddress);
    Task<(byte Status, int Latency)> PingWithMapperAsync(string ipAddress);
}