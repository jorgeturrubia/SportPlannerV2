using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.UseCases.Planning;

namespace SportPlanner.API.Controllers.Planning;

[ApiController]
[Route("api/planning/objectives")]
[Authorize]
public class ObjectivesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ObjectivesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new user objective
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateObjective(
        [FromBody] CreateObjectiveDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateObjectiveCommand(request);
        var objectiveId = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetObjective), new { id = objectiveId }, new { id = objectiveId });
    }

    /// <summary>
    /// Get objectives by subscription
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ObjectiveDto>>> GetObjectives(CancellationToken cancellationToken)
    {
        var query = new GetObjectivesBySubscriptionQuery();
        var objectives = await _mediator.Send(query, cancellationToken);

        return Ok(objectives);
    }

    /// <summary>
    /// Get a specific objective
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ObjectiveDto>> GetObjective([FromRoute] Guid id)
    {
        return Ok(new { message = "Get objective by ID - to be implemented" });
    }

    /// <summary>
    /// Update an existing objective
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateObjective(
        [FromRoute] Guid id,
        [FromBody] UpdateObjectiveDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateObjectiveCommand(request);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Delete an objective
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteObjective([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteObjectiveCommand(id);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Clone a system or marketplace objective to user's subscription
    /// </summary>
    [HttpPost("{id:guid}/clone")]
    public async Task<ActionResult<Guid>> CloneObjective([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new CloneObjectiveCommand(id);
        var clonedId = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetObjective), new { id = clonedId }, new { id = clonedId });
    }
}