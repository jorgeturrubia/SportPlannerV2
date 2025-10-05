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
        var query = new GetGendersQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create a new gender
    /// </summary>
    [HttpPost("genders")]
    public async Task<ActionResult<GenderResponse>> CreateGender(
        [FromBody] CreateGenderRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateGenderCommand(
            request.Name,
            request.Code,
            request.Description,
            request.IsActive ?? true
        );
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetGenders), new { }, result);
    }

    /// <summary>
    /// Update an existing gender
    /// </summary>
    [HttpPut("genders/{id:guid}")]
    public async Task<ActionResult<GenderResponse>> UpdateGender(
        [FromRoute] Guid id,
        [FromBody] UpdateGenderRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateGenderCommand(
            id,
            request.Name,
            request.Code,
            request.Description,
            request.IsActive ?? true
        );
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Delete a gender
    /// </summary>
    [HttpDelete("genders/{id:guid}")]
    public async Task<ActionResult> DeleteGender(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteGenderCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
        {
            return NotFound(new { message = "Gender not found" });
        }

        return NoContent();
    }

    /// <summary>
    /// Get age groups, optionally filtered by sport
    /// </summary>
    [HttpGet("age-groups")]
    public async Task<ActionResult<List<AgeGroupResponse>>> GetAgeGroups(
        [FromQuery] Sport? sport = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAgeGroupsQuery(sport);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create a new age group
    /// </summary>
    [HttpPost("age-groups")]
    public async Task<ActionResult<AgeGroupResponse>> CreateAgeGroup(
        [FromBody] CreateAgeGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateAgeGroupCommand(
            request.Name,
            request.Code,
            request.MinAge,
            request.MaxAge,
            request.Sport,
            request.SortOrder,
            request.IsActive ?? true
        );
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetAgeGroups), new { }, result);
    }

    /// <summary>
    /// Update an existing age group
    /// </summary>
    [HttpPut("age-groups/{id:guid}")]
    public async Task<ActionResult<AgeGroupResponse>> UpdateAgeGroup(
        [FromRoute] Guid id,
        [FromBody] UpdateAgeGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateAgeGroupCommand(
            id,
            request.Name,
            request.Code,
            request.MinAge,
            request.MaxAge,
            request.Sport,
            request.SortOrder,
            request.IsActive ?? true
        );
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Delete an age group
    /// </summary>
    [HttpDelete("age-groups/{id:guid}")]
    public async Task<ActionResult> DeleteAgeGroup(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteAgeGroupCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
        {
            return NotFound(new { message = "Age group not found" });
        }

        return NoContent();
    }

    /// <summary>
    /// Create a new team category
    /// </summary>
    [HttpPost("team-categories")]
    public async Task<ActionResult<TeamCategoryResponse>> CreateTeamCategory(
        [FromBody] CreateTeamCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateTeamCategoryCommand(
            request.Name,
            request.Code,
            request.Description,
            request.Sport,
            request.SortOrder,
            request.IsActive ?? true
        );
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetTeamCategories), new { }, result);
    }

    /// <summary>
    /// Update an existing team category
    /// </summary>
    [HttpPut("team-categories/{id:guid}")]
    public async Task<ActionResult<TeamCategoryResponse>> UpdateTeamCategory(
        [FromRoute] Guid id,
        [FromBody] UpdateTeamCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateTeamCategoryCommand(
            id,
            request.Name,
            request.Code,
            request.Description,
            request.Sport,
            request.SortOrder,
            request.IsActive ?? true
        );
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Delete a team category
    /// </summary>
    [HttpDelete("team-categories/{id:guid}")]
    public async Task<ActionResult> DeleteTeamCategory(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteTeamCategoryCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
        {
            return NotFound(new { message = "Team category not found" });
        }

        return NoContent();
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