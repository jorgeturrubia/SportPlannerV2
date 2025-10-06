using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.UseCases.Planning;
using SportPlanner.Domain.Enum;

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
        [FromQuery] Sport? sport = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetObjectiveCategoriesQuery(sport);
        var categories = await _mediator.Send(query, cancellationToken);

        return Ok(categories);
    }
}
