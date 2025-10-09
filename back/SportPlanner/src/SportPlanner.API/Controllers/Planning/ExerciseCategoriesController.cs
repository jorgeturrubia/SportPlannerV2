using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.UseCases.Planning;
using SportPlanner.Domain.Enum;

namespace SportPlanner.API.Controllers.Planning;

[ApiController]
[Route("api/planning/exercise-categories")]
[Authorize]
public class ExerciseCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExerciseCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get exercise categories, optionally filtered by sport
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ExerciseCategoryDto>>> GetCategories(
        [FromQuery] Sport? sport,
        CancellationToken cancellationToken)
    {
        var query = new GetExerciseCategoriesQuery(sport);
        var categories = await _mediator.Send(query, cancellationToken);

        return Ok(categories);
    }

    /// <summary>
    /// Create a new exercise category (System/Admin only - to be implemented with proper authorization)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateCategory(
        [FromBody] CreateExerciseCategoryDto request)
    {
        // TODO: Implement create command
        return Ok(new { message = "Create exercise category - to be implemented" });
    }

    /// <summary>
    /// Update an exercise category (System/Admin only - to be implemented with proper authorization)
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateCategory(
        [FromRoute] Guid id,
        [FromBody] UpdateExerciseCategoryDto request)
    {
        // TODO: Implement update command
        return NoContent();
    }

    /// <summary>
    /// Delete an exercise category (System/Admin only - to be implemented with proper authorization)
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteCategory([FromRoute] Guid id)
    {
        // TODO: Implement delete command
        return NoContent();
    }
}
