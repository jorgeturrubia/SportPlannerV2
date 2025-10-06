using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.UseCases.Planning;

namespace SportPlanner.API.Controllers.Planning;

[ApiController]
[Route("api/planning/objective-subcategories")]
[Authorize]
public class ObjectiveSubcategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ObjectiveSubcategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all objective subcategories, optionally filtered by category
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ObjectiveSubcategoryDto>>> GetSubcategories(
        [FromQuery] Guid? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetObjectiveSubcategoriesQuery(categoryId);
        var subcategories = await _mediator.Send(query, cancellationToken);

        return Ok(subcategories);
    }
}
