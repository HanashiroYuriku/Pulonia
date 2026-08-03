using System.Net.NetworkInformation;
using Pulonia.Application.Common.Interfaces.ExternalServices;

namespace Pulonia.Infrastructure.ExternalServices;

public class NetworkService : INetworkService
{
    // max timeout
    private readonly int _timeoutMs = 5000;
    // will send ping 3 times
    private readonly int _maxRetries = 3;

    public async Task<(bool IsSuccess, int Latency)> PingWithRetryAsync(string ipAddress)
    {
if (string.IsNullOrWhiteSpace(ipAddress)) return (false, 0);
        // set pinger
        using var pinger = new Ping();

        // send ping
        for (int attempt = 1; attempt <= _maxRetries; attempt++)
        {
            try
            {
                var reply = await pinger.SendPingAsync(ipAddress, _timeoutMs);

                // return if success
                if (reply.Status == IPStatus.Success)
                {
                    return (true, (int)reply.RoundtripTime);
                }
            }
            catch{}

            // delay per attempt
            if (attempt < _maxRetries) await Task.Delay(1000); // ms => 1 second
        }

        // return failed ping
        return (false, 0);
    }
}