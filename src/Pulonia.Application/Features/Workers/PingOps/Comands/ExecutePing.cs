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

// Represents a temporary holding structure for ping results during the cycle.
// Using a struct instead of a class prevents massive heap memory 
// allocations and reduces Garbage Collector (GC) overhead during high-volume parallel operations.
public record struct TempPingResult(
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
        // ==============================================================================
        // PREPARATION & DATA FETCHING
        // ==============================================================================
        
        // Fetch all active stores as read-only (AsNoTracking) to conserve memory.
        var activeStores = await _storeRepository.GetAllActiveStoresAsync(cancellationToken, trackChanges: false);
        if (activeStores == null || !activeStores.Any()) return;

        // Thread-safe collection to collect ping results from multiple concurrent threads.
        var tempPingResults = new ConcurrentBag<TempPingResult>();
        
        // Snap the exact UTC time to ensure all logs in this cycle share the exact same timestamp.
        var cycleCheckTime = DateTime.UtcNow;
        
        // Adjust SLA calculation boundary to the local TimeZone (WIB). 
        // This prevents SLA date rollover issues where UTC still considers it as "yesterday".
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(cycleCheckTime, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")));
        
        // Thread-safe dictionary serving as a local cache for this specific cycle.
        // Goal: Prevent duplicate network pings if multiple stores share the same Router IP.
        var routerPingCache = new ConcurrentDictionary<string, bool>();

        // Configure the concurrency limit to protect the OS from Socket Exhaustion.
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = _maxParallelism,
            CancellationToken = cancellationToken
        };


        // ==============================================================================
        // PARALLEL EXECUTION
        // ==============================================================================
        
        await Parallel.ForEachAsync(activeStores, parallelOptions, async (store, ct) =>
        {
            bool isRouterUp = true;
            bool isStoreUp = false;
            int storeLatency = 0;

            // --- EVALUATE ROUTER ---
            // Check if store have router
            if (!string.IsNullOrWhiteSpace(store.IpRouter))
            {
                // Check if another parallel thread has already pinged this exact Router IP in this cycle.
                if (routerPingCache.TryGetValue(store.IpRouter, out bool cachedRouterStatus))
                {
                    isRouterUp = cachedRouterStatus; // Use cached result, skip actual ping.
                }
                else
                {
                    // First time encountering this Router IP. Execute actual network ping.
                    var (successRouter, latencyRouter) = await _networkService.PingWithRetryAsync(store.IpRouter);
                    isRouterUp = successRouter;
                    
                    // Save the result to the cache for other threads to use.
                    routerPingCache.TryAdd(store.IpRouter, isRouterUp);
                    
                    tempPingResults.Add(new TempPingResult(
                        Ip: store.IpRouter, 
                        StoreCode: store.StoreCode, 
                        IpType: IpType.IpRouter, 
                        IsSuccess: isRouterUp, 
                        Latency: latencyRouter, 
                        CheckedAt: cycleCheckTime));
                }
            }

            // --- EVALUATE STORE MACHINE ---
            // Cross-Evaluation Logic: We only ping the Store IP if the Router is UP.
            // If the Router is DOWN, we assume the Store is disconnected (saves bandwidth and CPU time).
            if (isRouterUp && !string.IsNullOrWhiteSpace(store.IpAddress))
            {
                var (successStore, latencyStore) = await _networkService.PingWithRetryAsync(store.IpAddress);
                isStoreUp = successStore;
                storeLatency = latencyStore;
            }
            
            // --- RECORD FINAL STORE STATUS ---
            // If the router was down, isStoreUp remains false by default.
            tempPingResults.Add(new TempPingResult(
                Ip: store.IpAddress ?? string.Empty, 
                StoreCode: store.StoreCode, 
                IpType: IpType.IpAddress, 
                IsSuccess: isStoreUp, 
                Latency: storeLatency, 
                CheckedAt: cycleCheckTime));
        });


        // ==============================================================================
        // BULK INSERT LOGGING
        // ==============================================================================
        
        // BEST PRACTICE: Deferred Execution. 
        // We project the struct to the Entity class without calling .ToList() to save RAM.
        var logProjections = tempPingResults.Select(r => new StorePingLog
        {
            Ip = r.Ip, 
            StoreCode = r.StoreCode, 
            IpType = r.IpType, 
            IsSuccess = r.IsSuccess, 
            Latency = r.Latency, 
            CheckedAt = r.CheckedAt
        });

        // Split the massive insert into chunks of 5000 to prevent hitting SQL command length limits.
        foreach (var chunk in logProjections.Chunk(5000))
        {
            await _pingLogRepository.BulkInsertAsync(chunk.ToList(), cancellationToken);
        }


        // ==============================================================================
        // DAILY SLA AGGREGATION & UPSERT
        // ==============================================================================
        
        // Filter out Router IPs. We only calculate SLA based on the final Store IP status.
        var slaUpdates = tempPingResults
            .Where(r => r.IpType == IpType.IpAddress)
            // Group by StoreCode as a safeguard against duplicate master data from the database.
            // This prevents "Merge statement attempted to update same row" SQL exceptions.
            .GroupBy(r => r.StoreCode)
            .Select(g => new DailySla
            {   
                StoreCode = g.Key,
                Date = today,
                TotalPings = g.Count(), 
                SuccessPings = g.Count(x => x.IsSuccess)
            }).ToList();    

        // Send the raw incremental data (0 or 1) to the database. 
        // The actual daily accumulation (Current + New) happens inside the DB engine via Merge/Upsert.
        await _dailySlaRepository.UpsertDailySlaAsync(slaUpdates, cancellationToken);
    }
}