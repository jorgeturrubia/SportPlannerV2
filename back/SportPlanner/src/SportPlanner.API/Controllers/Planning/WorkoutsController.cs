using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.UseCases.Planning;

namespace SportPlanner.API.Controllers.Planning;

[ApiController]
[Route("api/planning/workouts")]
[Authorize]
public class WorkoutsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkoutsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new workout
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateWorkout(
        [FromBody] CreateWorkoutDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateWorkoutCommand(request);
        var workoutId = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetWorkout), new { id = workoutId }, new { id = workoutId });
    }

    /// <summary>
    /// Get workouts by subscription
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<WorkoutDto>>> GetWorkouts(CancellationToken cancellationToken)
    {
        var query = new GetWorkoutsBySubscriptionQuery();
        var workouts = await _mediator.Send(query, cancellationToken);

        return Ok(workouts);
    }

    /// <summary>
    /// Get a specific workout
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkoutDto>> GetWorkout([FromRoute] Guid id)
    {
        return Ok(new { message = "Get workout by ID - to be implemented" });
    }
}