using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.UseCases;
using SportPlanner.Domain.Enum;

namespace SportPlanner.API.Controllers;

[ApiController]
[Route("api/master-data")]
[Authorize]
public class MasterDataController : ControllerBase
{
    private readonly IMediator _mediator;

    public MasterDataController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get team categories, optionally filtered by sport
    /// </summary>
    [HttpGet("team-categories")]
    public async Task<ActionResult<List<TeamCategoryResponse>>> GetTeamCategories(
        [FromQuery] Sport? sport = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTeamCategoriesQuery(sport);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get all available genders
    /// </summary>
    [HttpGet("genders")]
    public async Task<ActionResult<List<GenderResponse>>> GetGenders(
        CancellationToken cancellationToken = default)
    {
        // For now, return a simple response - this would need a proper query handler
        return Ok(new { message = "Genders endpoint - to be implemented with GetGendersQuery" });
    }

    /// <summary>
    /// Get age groups, optionally filtered by sport
    /// </summary>
    [HttpGet("age-groups")]
    public async Task<ActionResult<List<AgeGroupResponse>>> GetAgeGroups(
        [FromQuery] Sport? sport = null,
        CancellationToken cancellationToken = default)
    {
        // For now, return a simple response - this would need a proper query handler
        return Ok(new { message = "Age groups endpoint - to be implemented with GetAgeGroupsQuery" });
    }

    /// <summary>
    /// Get all available team colors as enum values
    /// </summary>
    [HttpGet("team-colors")]
    public ActionResult<List<object>> GetTeamColors()
    {
        var colors = Enum.GetValues<TeamColor>()
            .Select(color => new
            {
                Id = (int)color,
                Name = color.ToString(),
                Value = color
            })
            .ToList();

        return Ok(colors);
    }
}