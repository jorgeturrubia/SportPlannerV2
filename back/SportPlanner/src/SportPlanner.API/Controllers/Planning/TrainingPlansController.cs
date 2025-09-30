using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.UseCases.Planning;

namespace SportPlanner.API.Controllers.Planning;

[ApiController]
[Route("api/planning/training-plans")]
[Authorize]
public class TrainingPlansController : ControllerBase
{
    private readonly IMediator _mediator;

    public TrainingPlansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new training plan
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CreatePlan(
        [FromBody] CreateTrainingPlanDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTrainingPlanCommand(request);
        var planId = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetPlan), new { id = planId }, new { id = planId });
    }

    /// <summary>
    /// Get training plans by subscription
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TrainingPlanDto>>> GetPlans(CancellationToken cancellationToken)
    {
        var query = new GetTrainingPlansBySubscriptionQuery();
        var plans = await _mediator.Send(query, cancellationToken);

        return Ok(plans);
    }

    /// <summary>
    /// Get a specific training plan
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TrainingPlanDto>> GetPlan([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetTrainingPlanByIdQuery(id);
        var plan = await _mediator.Send(query, cancellationToken);

        return Ok(plan);
    }

    /// <summary>
    /// Update a training plan
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdatePlan(
        [FromRoute] Guid id,
        [FromBody] UpdateTrainingPlanDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTrainingPlanCommand(request);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Add an objective to a training plan
    /// </summary>
    [HttpPost("{id:guid}/objectives")]
    public async Task<ActionResult> AddObjective(
        [FromRoute] Guid id,
        [FromBody] AddObjectiveToPlanRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddObjectiveToPlanCommand(id, request.ObjectiveId, request.Priority, request.TargetSessions);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}

public class AddObjectiveToPlanRequest
{
    public Guid ObjectiveId { get; set; }
    public int Priority { get; set; }
    public int TargetSessions { get; set; }
}