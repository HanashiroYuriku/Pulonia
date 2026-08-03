using Asp.Versioning;
using Pulonia.Application.Features.Users.Commands;
using Pulonia.Application.Features.Users.Queries;
using Pulonia.Domain.Enums;
using Cortex.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pulonia.Api.Controllers.v1;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class UserController : ControllerBase
{
    // Inject Mediator
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // This end point isn't self register for user
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.SendAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result }, new { id = result });
    }

    [Authorize(Roles = nameof(Role.Admin))]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var query = new GetUserByIdQuery(id);

        var result = await _mediator.QueryAsync(query, ct);

        return Ok(result);
    }
}