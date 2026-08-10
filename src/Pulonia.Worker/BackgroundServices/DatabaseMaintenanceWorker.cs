
using Cortex.Mediator;
using Pulonia.Application.Features.Workers.Maintenance.Commands;

namespace Pulonia.Worker.BackgroundServices;

public class DatabaseMaintenanceWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseMaintenanceWorker> _logger;

    // execeution time at 01.00 UTC
    private readonly TimeSpan _executionTime = new(1, 0, 0);

    public DatabaseMaintenanceWorker(IServiceProvider serviceProvider, ILogger<DatabaseMaintenanceWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = CalculateDelayToNextExecution();
            _logger.LogInformation("Database Maintenance Worker sleep {DelayHours} Hours", delay.TotalHours);

            await Task.Delay(delay, stoppingToken);

            if (stoppingToken.IsCancellationRequested) break;
            _logger.LogInformation("Starting Maintenance");

            try
            {
                using var scoop = _serviceProvider.CreateScope();
                var mediator = scoop.ServiceProvider.GetRequiredService<IMediator>();

                await mediator.SendAsync(new RotatePingLogPartition(7), stoppingToken);

                _logger.LogInformation("Success Maintain Partition");
            }
            catch (Exception e)
            {

                _logger.LogError(e, "Failed to Maintain Partition");
            }
        }
    }

    private TimeSpan CalculateDelayToNextExecution()
    {
        var now = DateTime.UtcNow;
        var nextRun = now.Date + _executionTime;

        if (now > nextRun)
        {
            nextRun = nextRun.AddDays(1);
        }

        return nextRun - now;
    }
}