using Cortex.Mediator.Commands;
using Pulonia.Application.Common.Interfaces.Repositories;

namespace Pulonia.Application.Features.Workers.Maintenance.Commands;

public record RotatePingLogPartition(int RetentionDays) : ICommand;

public class RotatePingLogPartitionHandler : ICommandHandler<RotatePingLogPartition>
{
    private readonly IPingLogRepository _pingLogRepository;

    public RotatePingLogPartitionHandler(IPingLogRepository pingLogRepository)
    {
        _pingLogRepository = pingLogRepository;
    }

    public async Task Handle(RotatePingLogPartition command, CancellationToken cancellationToken)
    {
        await _pingLogRepository.RotatePartitionsASync(command.RetentionDays, cancellationToken);
    }
}