using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.UseCases;

namespace SportPlanner.API.Controllers;

[ApiController]
[Route("api/subscriptions/{subscriptionId:guid}/teams")]
[Authorize]
public class TeamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeamsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new team within a subscription
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateTeam(
        [FromRoute] Guid subscriptionId,
        [FromBody] CreateTeamRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTeamCommand(
            subscriptionId,
            request.Name,
            request.Color,
            request.TeamCategoryId,
            request.GenderId,
            request.AgeGroupId,
            request.Description,
            request.HomeVenue,
            request.CoachName,
            request.ContactEmail,
            request.ContactPhone,
            request.Season,
            request.AllowMixedGender);

        var teamId = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetTeam),
            new { subscriptionId, teamId },
            new { id = teamId });
    }

    /// <summary>
    /// Get a specific team by ID
    /// </summary>
    [HttpGet("{teamId:guid}")]
    public async Task<ActionResult<TeamResponse>> GetTeam(
        [FromRoute] Guid subscriptionId,
        [FromRoute] Guid teamId,
        CancellationToken cancellationToken)
    {
        // For now, return a simple response - this would need a proper query handler
        return Ok(new { message = "Team endpoint - to be implemented with GetTeamQuery" });
    }

    /// <summary>
    /// Get all teams for a subscription
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TeamResponse>>> GetTeams(
        [FromRoute] Guid subscriptionId,
        [FromQuery] bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        // For now, return a simple response - this would need a proper query handler
        return Ok(new { message = "Teams list endpoint - to be implemented with GetTeamsQuery" });
    }
}