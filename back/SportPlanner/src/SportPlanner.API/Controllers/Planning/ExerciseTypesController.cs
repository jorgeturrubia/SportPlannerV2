using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.UseCases.Planning;

namespace SportPlanner.API.Controllers.Planning;

[ApiController]
[Route("api/planning/exercise-types")]
[Authorize]
public class ExerciseTypesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExerciseTypesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all exercise types
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ExerciseTypeDto>>> GetTypes(CancellationToken cancellationToken)
    {
        var query = new GetExerciseTypesQuery();
        var types = await _mediator.Send(query, cancellationToken);

        return Ok(types);
    }

    /// <summary>
    /// Create a new exercise type (System/Admin only - to be implemented with proper authorization)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateType(
        [FromBody] CreateExerciseTypeDto request)
    {
        // TODO: Implement create command
        return Ok(new { message = "Create exercise type - to be implemented" });
    }

    /// <summary>
    /// Update an exercise type (System/Admin only - to be implemented with proper authorization)
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateType(
        [FromRoute] Guid id,
        [FromBody] UpdateExerciseTypeDto request)
    {
        // TODO: Implement update command
        return NoContent();
    }

    /// <summary>
    /// Delete an exercise type (System/Admin only - to be implemented with proper authorization)
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteType([FromRoute] Guid id)
    {
        // TODO: Implement delete command
        return NoContent();
    }
}
