using System.Collections.Concurrent;
using Cortex.Mediator.Commands;
using Microsoft.Extensions.Options;
using Pulonia.Application.Common.Configurations;
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
    private readonly IDailySlaRepository _dailySlaRepository;
    private readonly int _maxParallelism;

    public ExecutePingHandler(
        IStoreRepository storeRepository, 
        IPingLogRepository pingLogRepository, 
        INetworkService networkService,
        IDailySlaRepository dailySlaRepository,
        IOptions<PingConfiguration> pingOptions)
    {
        _storeRepository = storeRepository;
        _pingLogRepository = pingLogRepository;
        _networkService = networkService;
        _dailySlaRepository = dailySlaRepository;
        _maxParallelism = pingOptions.Value.MaxDegreeOfParallelism;
    }

    public async Task Handle(ExecutePing command, CancellationToken cancellationToken)
    {
        // 1. Fetch all active stores
        var activeStores = await _storeRepository.GetAllActiveStoresAsync(cancellationToken, trackChanges: false);
        
        var tempPingResults = new ConcurrentBag<TempPingResult>();
        var cycleCheckTime = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(cycleCheckTime.Date);
        
        // 2. Concurrency limit to prevent socket exhaustion on the OS
        using var semaphore = new SemaphoreSlim(_maxParallelism);

        // 3. Initialize a thread-safe dictionary to cache Router IPs for THIS cycle only.
        // Key: Router IP Address, Value: IsSuccess (True/False)
        var routerPingCache = new ConcurrentDictionary<string, bool>();

        var tasks = activeStores.Select(async store =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                bool isRouterUp = true;
                bool isStoreUp = false;
                int storeLatency = 0;

                // --- PHASE 1: EVALUATE ROUTER (WITH CACHING MECHANISM) ---
                if (!string.IsNullOrWhiteSpace(store.IpRouter))
                {
                    // Check if this Router IP has already been evaluated by another concurrent task
                    if (routerPingCache.TryGetValue(store.IpRouter, out bool cachedRouterStatus))
                    {
                        // Use the cached status. No need to send an actual ICMP Ping.
                        isRouterUp = cachedRouterStatus;
                    }
                    else
                    {
                        // First time seeing this Router IP in this cycle. Send actual Ping.
                        var (successRouter, latencyRouter) = await _networkService.PingWithRetryAsync(store.IpRouter);
                        isRouterUp = successRouter;
                        
                        // Store the result in the dictionary for other tasks
                        routerPingCache.TryAdd(store.IpRouter, isRouterUp);
                        
                        // Log the Router ping result
                        tempPingResults.Add(new TempPingResult(
                            Ip: store.IpRouter, 
                            StoreCode: store.StoreCode, 
                            IpType: IpType.IpRouter, 
                            IsSuccess: isRouterUp, 
                            Latency: latencyRouter, 
                            CheckedAt: cycleCheckTime));
                    }
                }

                // --- PHASE 2: EVALUATE STORE MACHINE ---
                // Business Rule: Only ping the Store IP if the Router is UP (or if there is no Router IP at all).
                if (isRouterUp)
                {
                    var (successStore, latencyStore) = await _networkService.PingWithRetryAsync(store.IpAddress);
                    isStoreUp = successStore;
                    storeLatency = latencyStore;
                }
                
                // --- PHASE 3: RECORD FINAL STORE STATUS ---
                // Note: If Router is down, isStoreUp remains false by default (preventing false positives).
                tempPingResults.Add(new TempPingResult(
                    Ip: store.IpAddress, 
                    StoreCode: store.StoreCode, 
                    IpType: IpType.IpAddress, 
                    IsSuccess: isStoreUp, 
                    Latency: storeLatency, 
                    CheckedAt: cycleCheckTime));
            }
            finally
            {
                semaphore.Release();
            }
        });

        // 4. Wait for all 25,000 parallel executions to finish
        await Task.WhenAll(tasks);

        // 5. Bulk Insert Logging
        var allLogs = tempPingResults.Select(r => new StorePingLog
        {
            Ip = r.Ip, 
            StoreCode = r.StoreCode, 
            IpType = r.IpType, 
            IsSuccess = r.IsSuccess, 
            Latency = r.Latency, 
            CheckedAt = r.CheckedAt
        }).ToList();

        // Chunking is crucial to prevent SQL command size limits
        foreach (var chunk in allLogs.Chunk(5000))
        {
            await _pingLogRepository.BulkInsertAsync(chunk, cancellationToken);
        }

        // 6. Aggregate SLA Calculation
        // We only aggregate SLA based on the final representation of the Store Machine (IpType.IpAddress)
        var slaUpdates = tempPingResults
            .Where(r => r.IpType == IpType.IpAddress)
            .Select(r => new DailySla
            {   
                StoreCode = r.StoreCode,
                Date = today,
                TotalPings = 1,
                SuccessPings = r.IsSuccess ? 1 : 0
            }).ToList();    

        await _dailySlaRepository.UpsertDailySlaAsync(slaUpdates, cancellationToken);
    }
}