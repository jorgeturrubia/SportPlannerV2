using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.UseCases.Planning;

namespace SportPlanner.API.Controllers.Planning;

[ApiController]
[Route("api/planning/exercises")]
[Authorize]
public class ExercisesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExercisesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new exercise
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateExercise(
        [FromBody] CreateExerciseDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateExerciseCommand(request);
        var exerciseId = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetExercise), new { id = exerciseId }, new { id = exerciseId });
    }

    /// <summary>
    /// Get exercises by subscription
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ExerciseDto>>> GetExercises(CancellationToken cancellationToken)
    {
        var query = new GetExercisesBySubscriptionQuery();
        var exercises = await _mediator.Send(query, cancellationToken);

        return Ok(exercises);
    }

    /// <summary>
    /// Get a specific exercise
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExerciseDto>> GetExercise([FromRoute] Guid id)
    {
        return Ok(new { message = "Get exercise by ID - to be implemented" });
    }

    /// <summary>
    /// Clone a system or marketplace exercise
    /// </summary>
    [HttpPost("{id:guid}/clone")]
    public async Task<ActionResult<Guid>> CloneExercise([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new CloneExerciseCommand(id);
        var clonedId = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetExercise), new { id = clonedId }, new { id = clonedId });
    }
}