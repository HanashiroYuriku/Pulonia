using System.Collections.Concurrent;
using Cortex.Mediator.Commands;
using Microsoft.Extensions.Options;
using Pulonia.Application.Common.Configurations;
using Pulonia.Application.Common.Interfaces.ExternalServices;
using Pulonia.Application.Common.Interfaces.Repositories;
using Pulonia.Domain.Entities;
using Pulonia.Domain.Enums;

namespace Pulonia.Application.Features.Workers.PingOps.Commands;

public record ExecutePing : ICommand;

// Represents a temporary holding structure for ping results during the cycle.
// Using a struct instead of a class prevents massive heap memory 
// allocations and reduces Garbage Collector (GC) overhead during high-volume parallel operations.
public record struct TempPingResult(
    int StoreId,
    string IpAddress,
    byte IpType,
    byte PingStatus,
    int Latency,
    DateTime CheckedAt);

public record struct TempStoreSummary(
    int StoreId,
    byte FinalCurrentStatus,
    bool IsSlaSuccess,
    int LowestLatency);

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
        var activeStores = await _storeRepository.GetAllActiveStoresAsync(cancellationToken, trackChanges: false, ignoreQueryFilters: true);
        if (activeStores == null || !activeStores.Any()) return;

        var tempPingResults = new ConcurrentBag<TempPingResult>();
        var tempStoreSummaries = new ConcurrentBag<TempStoreSummary>(); // Penampung status final

        var cycleCheckTime = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(cycleCheckTime, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")));
        var routerPingCache = new ConcurrentDictionary<string, byte>(); // Cache sekarang menyimpan PingStatus (byte)

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = _maxParallelism,
            CancellationToken = cancellationToken
        };

        // ==============================================================================
        // PARALLEL EXECUTION (100.000 Pings Target)
        // ==============================================================================
        await Parallel.ForEachAsync(activeStores, parallelOptions, async (store, ct) =>
        {
            // 1. --- EVALUASI MAIN LINE (UTAMA) ---
            byte mainRouterStatus = 4; // Default Error
            int mainRouterLatency = 0;
            byte mainIpStatus = 4;
            int mainIpLatency = 0;

            // Ping Main Router (dengan mekanisme Cache)
            if (!string.IsNullOrWhiteSpace(store.MainIpRouter))
            {
                if (routerPingCache.TryGetValue(store.MainIpRouter, out byte cachedStatus))
                {
                    mainRouterStatus = cachedStatus;
                }
                else
                {
                    // Gunakan class PingStatusMapper yang sudah kita bahas sebelumnya
                    var (status, latency) = await _networkService.PingWithMapperAsync(store.MainIpRouter);
                    mainRouterStatus = status;
                    mainRouterLatency = latency;
                    routerPingCache.TryAdd(store.MainIpRouter, mainRouterStatus);

                    tempPingResults.Add(new TempPingResult(store.Id, store.MainIpRouter, 1, mainRouterStatus, mainRouterLatency, cycleCheckTime));
                }
            }

            // Ping Main IP (Hanya jika Router Utama UP - Hemat Resource!)
            if (mainRouterStatus == 1 && !string.IsNullOrWhiteSpace(store.MainIpAddress))
            {
                var (status, latency) = await _networkService.PingWithMapperAsync(store.MainIpAddress);
                mainIpStatus = status;
                mainIpLatency = latency;

                tempPingResults.Add(new TempPingResult(store.Id, store.MainIpAddress, 2, mainIpStatus, mainIpLatency, cycleCheckTime));
            }


            // 2. --- EVALUASI BACKUP LINE (CADANGAN) ---
            byte backupRouterStatus = 4;
            int backupRouterLatency = 0;
            byte backupIpStatus = 4;
            int backupIpLatency = 0;
            bool hasBackup = !string.IsNullOrWhiteSpace(store.BackupIpRouter);

            if (hasBackup)
            {
                if (routerPingCache.TryGetValue(store.BackupIpRouter, out byte cachedBckStatus))
                {
                    backupRouterStatus = cachedBckStatus;
                }
                else
                {
                    var (status, latency) = await _networkService.PingWithMapperAsync(store.BackupIpRouter);
                    backupRouterStatus = status;
                    backupRouterLatency = latency;
                    routerPingCache.TryAdd(store.BackupIpRouter, backupRouterStatus);

                    tempPingResults.Add(new TempPingResult(store.Id, store.BackupIpRouter, 3, backupRouterStatus, backupRouterLatency, cycleCheckTime));
                }

                // Ping Backup IP (Hanya jika Backup Router UP)
                if (backupRouterStatus == 1 && !string.IsNullOrWhiteSpace(store.BackupIpAddress))
                {
                    var (status, latency) = await _networkService.PingWithMapperAsync(store.BackupIpAddress);
                    backupIpStatus = status;
                    backupIpLatency = latency;

                    tempPingResults.Add(new TempPingResult(store.Id, store.BackupIpAddress, 4, backupIpStatus, backupIpLatency, cycleCheckTime));
                }
            }


            // 3. --- PENENTUAN STATUS AKHIR TOKO & SLA ---
            bool isSlaSuccess = false;
            byte finalState = 0; // 0: Down (Default)
            int lowestLatency = 0;

            // Evaluasi Kasta Jaringan
            if (mainRouterStatus == 1)
            {
                isSlaSuccess = true;
                lowestLatency = mainRouterLatency; // Utamakan latency jalan tol

                if (mainIpStatus == 1) finalState = 1; // Sempurna (Up)
                else finalState = 2; // Warning Main Ip Down (Lan Putus)
            }
            else if (hasBackup && backupRouterStatus == 1)
            {
                isSlaSuccess = true;
                lowestLatency = backupRouterLatency; // Pakai latency VSAT

                if (backupIpStatus == 1) finalState = 4; // Warning Backup Ip Down
                else finalState = 5; // Warning Backup Router Down
            }

            // Simpan konklusi toko ini ke bag
            tempStoreSummaries.Add(new TempStoreSummary(store.Id, finalState, isSlaSuccess, lowestLatency));
        });


        // ==============================================================================
        // BULK INSERT LOGGING (Raw Pings)
        // ==============================================================================
        var logProjections = tempPingResults.Select(r => new StorePingLog
        {
            StoreId = r.StoreId,
            CheckedAt = r.CheckedAt,
            IpType = (IpType)r.IpType,
            Ip = r.IpAddress,
            PingStatus = (PingStatus)r.PingStatus,
            Latency = r.Latency
        });

        foreach (var chunk in logProjections.Chunk(5000))
        {
            await _pingLogRepository.BulkInsertAsync(chunk.ToList(), cancellationToken);
        }


        // ==============================================================================
        // DAILY SLA AGGREGATION & UPSERT
        // ==============================================================================
        // Kita bangun SLA dari kesimpulan toko, bukan dari mentahan IP lagi
        var slaUpdates = tempStoreSummaries
            .Select(s => new DailySla
            {
                StoreId = s.StoreId,
                Date = today,
                TotalPings = 1, // 1 siklus = 1 ping untuk toko ini
                SuccessPings = s.IsSlaSuccess ? 1 : 0,
                MinimumLatency = s.LowestLatency,
                MaximumLatency = s.LowestLatency,
                TotalLatency = s.LowestLatency
            }).ToList();

        await _dailySlaRepository.UpsertDailySlaAsync(slaUpdates, cancellationToken);


        // ==============================================================================
        // STORE STATES & INCIDENT MANAGEMENT (Tugas Baru)
        // ==============================================================================
        // TODO: Kirim `tempStoreSummaries` ke Repositori State/Incident 
        // Pastikan menggunakan mekanisme BulkUpdate (ExecuteUpdateAsync) karena melakukan 
        // update state satu per satu untuk 25k toko akan memakan waktu terlalu lama.

        // await _storeStateRepository.ProcessStatesAndIncidentsAsync(tempStoreSummaries.ToList(), cancellationToken);
    }
}