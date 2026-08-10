using System.Diagnostics;
using Cortex.Mediator;
using Pulonia.Application.Features.Workers.PingOps.Comands;
using Serilog.Context;

namespace Pulonia.Worker.BackgroundServices;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Pulonia's Worker: Started at {time}", DateTimeOffset.Now);

        // Set the timer to trigger every 5 minutes
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

        try
        {
            await DoWorkAsync(stoppingToken);

            // Infinite loop attached to the timer
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await DoWorkAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Pulonia's Worker: Gracefully shut down.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Pulonia's Worker: A fatal error occurred in the execution loop.");
        }
    }

    private async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        // Generate ID unik untuk melacak keseluruhan kejadian dalam satu siklus
        var cycleId = Guid.NewGuid().ToString("N");
        var sw = Stopwatch.StartNew();

        // Menyuntikkan CycleId ke semua log yang dieksekusi di dalam scope ini
        using (LogContext.PushProperty("CycleId", cycleId))
        using (LogContext.PushProperty("WorkerName", "PingPatrolWorker"))
        {
            _logger.LogInformation("Initiating mass ping cycle for all active stores");

            using (var scope = _scopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                try
                {
                    await mediator.SendAsync(new ExecutePing(), stoppingToken);

                    sw.Stop();
                    _logger.LogInformation("Mass ping cycle executed successfully in {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    _logger.LogError(ex, "Failed to execute ping cycle after {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                }
            }
        }
    }
}