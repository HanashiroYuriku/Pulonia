using Asp.Versioning;
using Cortex.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pulonia.Application.Features.DailySlaApi.Queries;

namespace Pulonia.Api.Controllers.v1;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class DailySlaController : ControllerBase
{
    private readonly IMediator _mediator;

    public DailySlaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetDailySla([FromQuery] DailySlaQuery query, CancellationToken cancellationToken)
    {
        if (query.Date == default)
        {
            query = query with { Date = DateOnly.FromDateTime(DateTime.UtcNow) };
        }

        var result = await _mediator.QueryAsync(query, cancellationToken);

        return Ok(result);
    }
}