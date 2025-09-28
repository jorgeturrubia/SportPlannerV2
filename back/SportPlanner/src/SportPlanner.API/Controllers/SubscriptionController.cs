using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.UseCases;

namespace SportPlanner.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubscriptionController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubscriptionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionRequest request)
    {
        var command = new CreateSubscriptionCommand(request.Type, request.Sport);
        var subscriptionId = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetMySubscription), new { id = subscriptionId }, subscriptionId);
    }

    [HttpGet("my")]
    [ProducesResponseType(typeof(SubscriptionResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetMySubscription()
    {
        var query = new GetUserSubscriptionQuery();
        var subscription = await _mediator.Send(query);

        return subscription is not null ? Ok(subscription) : NotFound();
    }

    [HttpPost("{subscriptionId}/users")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> AddUserToSubscription(
        Guid subscriptionId,
        [FromBody] AddUserToSubscriptionRequest request)
    {
        var command = new AddUserToSubscriptionCommand(subscriptionId, request);
        await _mediator.Send(command);

        return NoContent();
    }
}
