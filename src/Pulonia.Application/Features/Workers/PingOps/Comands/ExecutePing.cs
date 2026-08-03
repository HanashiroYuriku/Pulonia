    using System.Collections.Concurrent;
    using Cortex.Mediator;
    using Cortex.Mediator.Commands;
    using Pulonia.Application.Common.Interfaces.ExternalServices;
    using Pulonia.Application.Common.Interfaces.Repositories;
    using Pulonia.Domain.Entities;
    using Pulonia.Domain.Enums;

    namespace Pulonia.Application.Features.Workers.PingOps.Comands;

    public record ExecutePing : ICommand;

    public record TempPingResult(
        string Ip, 
        string StoreCode, 
        IpType IpType, 
        bool IsSuccess, 
        int Latency, 
        DateTime CheckedAt);

    public class ExecutePingHandler : ICommandHandler<ExecutePing>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IPingLogRepository _pingLogRepository;
        private readonly INetworkService _networkService;

        public ExecutePingHandler(
            IStoreRepository storeRepository, 
            IPingLogRepository pingLogRepository, 
            INetworkService networkService
        )
        {
            _storeRepository = storeRepository;
            _pingLogRepository = pingLogRepository;
            _networkService = networkService;
        }
        public async Task Handle(ExecutePing command, CancellationToken cancellationToken)
        {
            // Get all active stores from the repository
            var activeStores = await _storeRepository.GetAllActiveStoresAsync(cancellationToken, trackChanges: false);

            // Create a concurrent bag to store the ping results
            var tempPingResults = new ConcurrentBag<TempPingResult>();

            // Snap the current time to use as the checked time for all ping results
            var cycleCheckTime = DateTime.UtcNow;

            // Create a semaphore to limit the number of concurrent ping operations
            var semaphore = new SemaphoreSlim(500); // limit 500 sockets opened at the same time

            var tasks = activeStores.Select(async store =>
            {
                await semaphore.WaitAsync(cancellationToken);
                try
                {
                    // Ping the branch's IP address with retry logic
                    var (isSuccess, latency) = await _networkService.PingWithRetryAsync(store.IpAddress);

                    // Add the ping result to the concurrent bag
                    tempPingResults.Add(
                        new TempPingResult(
                            Ip: store.IpAddress,
                            StoreCode: store.StoreCode,
                            IpType: IpType.IpAddress,
                            IsSuccess: isSuccess,
                            Latency: latency,
                            CheckedAt: cycleCheckTime
                        )
                    );

                    // If the ping was successful and the store has a router IP, ping the router IP as well
                    if (isSuccess && !string.IsNullOrWhiteSpace(store.IpRouter))
                    {
                        var (isSuccessRouter, latencyRouter) = await _networkService.PingWithRetryAsync(store.IpRouter);
                        tempPingResults.Add(
                            new TempPingResult(
                                Ip: store.IpRouter,
                                StoreCode: store.StoreCode,
                                IpType: IpType.IpRouter,
                                IsSuccess: isSuccessRouter,
                                Latency: latencyRouter,
                                CheckedAt: cycleCheckTime
                            )
                        );
                    }
                }
                finally
                {
                    semaphore.Release();
                }

            });

            // Wait for all ping tasks to complete
            await Task.WhenAll(tasks);

            var allLogs = tempPingResults.Select(r => new StorePingLog
            {
                Ip = r.Ip,
                StoreCode = r.StoreCode,
                IpType = r.IpType,
                IsSuccess = r.IsSuccess,
                Latency = r.Latency,
                CheckedAt = r.CheckedAt
            }).ToList();

            foreach (var chunk in allLogs.Chunk(5000))
            {
                await _pingLogRepository.BulkInsertAsync(chunk, cancellationToken);
            }
        }
    }