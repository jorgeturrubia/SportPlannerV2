using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportPlanner.Application.Dtos.Planning;
using SportPlanner.Application.UseCases.Planning;
using SportPlanner.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportPlanner.API.Controllers.Planning;

[ApiController]
[Authorize] // Authorization can be more granular, e.g., Admin-only for create/update
[Route("api/planning/itineraries")]
public class ItineraryController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItineraryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Request model for creating an itinerary
    public record CreateItineraryRequest(
        string Name,
        string Description,
        Sport Sport,
        Difficulty Level,
        List<SportPlanner.Application.UseCases.Planning.ItineraryItemToAdd> Items);

    /// <summary>
    /// Gets all active itineraries.
    /// </summary>
    [HttpGet]
    [AllowAnonymous] // Allow all users to see the list of itineraries
    [ProducesResponseType(typeof(IReadOnlyCollection<ItineraryDto>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetItinerariesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new itinerary. (Admin-only endpoint)
    /// </summary>
    [HttpPost]
    // [Authorize(Roles = "Admin")] // Example of more specific authorization
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateItineraryRequest request)
    {
        var command = new CreateItineraryCommand(
            request.Name,
            request.Description,
            request.Sport,
            request.Level,
            request.Items);

        var newItineraryId = await _mediator.Send(command);

        // This assumes a GetById endpoint exists, which would be the next step for full CRUD
        // For now, we return the ID in the body.
        return StatusCode(201, newItineraryId);
    }
}