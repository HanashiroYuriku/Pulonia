using Asp.Versioning;
using Cortex.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pulonia.Application.Features.Workers.PingOps.Commands;

namespace Pulonia.Api.Controllers.v1;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class WorkerController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // [Authorize]
    [HttpPost]
    public async Task<IActionResult> ExecuteWorker([FromQuery] ExecutePing query, CancellationToken cancellationToken)
    {
        await _mediator.SendAsync(query, cancellationToken);
        Console.WriteLine("Worker executed successfully.");
        return NoContent();
    }
}