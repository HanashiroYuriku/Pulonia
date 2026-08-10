using Pulonia.Application.Common.Interfaces.Repositories;
using Cortex.Mediator.Queries;
using Pulonia.Application.Common.Models;

namespace Pulonia.Application.Features.DailySlaApi.Queries;

// DTO - Represent Response
public record DailySlaResponse(
    string StoreCode,
    int TotalPings,
    int SuccessPings,
    decimal SlaPercentage
);

// Query
public record DailySlaQuery(
    DateOnly Date
) : PaginationParams, IQuery<PagedList<DailySlaResponse>>;

// Handler
public class DailySlaQueryHandler : IQueryHandler<DailySlaQuery, PagedList<DailySlaResponse>>
{
    private readonly IDailySlaRepository _dailySlaRepository;

    public DailySlaQueryHandler(IDailySlaRepository dailySlaRepository)
    {
        _dailySlaRepository = dailySlaRepository;
    }

    public async Task<PagedList<DailySlaResponse>> Handle(DailySlaQuery request, CancellationToken cancellationToken)
    {
        // Find user by id. trackChanges false for read only data
        var result = await _dailySlaRepository.GetSlaByDateAsync(
            request,
            request.Date,
            cancellationToken,
            trackChanges: false
        );

        var response = result.Items.Select(sla => new DailySlaResponse(
            StoreCode: sla.StoreCode,
            TotalPings: sla.TotalPings,
            SuccessPings: sla.SuccessPings,
            SlaPercentage: sla.SlaPercentage
        )).ToList();

        return new PagedList<DailySlaResponse>(
            items: response,
            totalCount: result.TotalCount,
            pageNumber: result.PageNumber,
            pageSize: result.PageSize);
    }
}