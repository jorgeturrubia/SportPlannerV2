using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.UseCases.Planning;

namespace SportPlanner.API.Controllers.Planning;

[ApiController]
[Route("api/planning/objective-categories")]
[Authorize]
public class ObjectiveCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ObjectiveCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all objective categories, optionally filtered by sport
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ObjectiveCategoryDto>>> GetCategories(
        [FromQuery] Guid? sportId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetObjectiveCategoriesQuery(sportId);
        var categories = await _mediator.Send(query, cancellationToken);

        return Ok(categories);
    }

    /// <summary>
    /// Create a new objective category (System/Admin only)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateCategory(
        [FromBody] CreateObjectiveCategoryDto request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateObjectiveCategoryCommand(request);
        var categoryId = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetCategories), new { id = categoryId }, categoryId);
    }

    /// <summary>
    /// Update an objective category (System/Admin only - to be implemented with proper authorization)
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateCategory(
        [FromRoute] Guid id,
        [FromBody] UpdateObjectiveCategoryDto request)
    {
        // TODO: Implement update command
        return NoContent();
    }

    /// <summary>
    /// Delete an objective category (System/Admin only - to be implemented with proper authorization)
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteCategory([FromRoute] Guid id)
    {
        // TODO: Implement delete command
        return NoContent();
    }
}
