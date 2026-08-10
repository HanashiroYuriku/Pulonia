using System.Net.NetworkInformation;
using Microsoft.Extensions.Options;
using Pulonia.Application.Common.Configurations;
using Pulonia.Application.Common.Interfaces.ExternalServices;

namespace Pulonia.Infrastructure.ExternalServices;

public class NetworkService : INetworkService
{
    private readonly int _timeoutMs;
    private readonly int _maxRetries;

    public NetworkService(IOptions<PingConfiguration> pingConfig)
    {
        _timeoutMs = pingConfig.Value.TimeoutMs;
        _maxRetries = pingConfig.Value.MaxRetries;
    }

    public async Task<(bool IsSuccess, int Latency)> PingWithRetryAsync(string ipAddress)
    {
        // Fail-safe for invalid IP inputs from the database
        if (string.IsNullOrWhiteSpace(ipAddress)) return (false, 0);

        using var pinger = new Ping();

        for (int attempt = 1; attempt <= _maxRetries; attempt++)
        {
            try
            {
                var reply = await pinger.SendPingAsync(ipAddress, _timeoutMs);

                if (reply.Status == IPStatus.Success)
                {
                    return (true, (int)reply.RoundtripTime);
                }
            }
            catch (PingException)
            {
                // We only swallow PingException (e.g., unreachable host during SendPingAsync).
                // Fatal exceptions like SocketException or OutOfMemoryException will bubble up 
                // and be caught by the Worker's top-level try-catch block for proper logging.
            }

            // Apply delay before the next retry, but not after the final failed attempt
            if (attempt < _maxRetries)
            {
                await Task.Delay(1000);
            }
        }

        // Return false if all retries are exhausted
        return (false, 0);
    }
}