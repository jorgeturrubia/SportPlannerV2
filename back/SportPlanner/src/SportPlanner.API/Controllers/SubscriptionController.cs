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
        try
        {
            Console.WriteLine($"[SubscriptionController] CreateSubscription called - Type: {request.Type}, Sport: {request.Sport}");
            Console.WriteLine($"[SubscriptionController] User.Identity.IsAuthenticated: {User.Identity?.IsAuthenticated}");
            Console.WriteLine($"[SubscriptionController] User.Identity.Name: {User.Identity?.Name}");

            var command = new CreateSubscriptionCommand(request.Type, request.Sport.Id);
            var subscriptionId = await _mediator.Send(command);

            Console.WriteLine($"[SubscriptionController] Subscription created successfully: {subscriptionId}");
            return CreatedAtAction(nameof(GetMySubscription), new { id = subscriptionId }, subscriptionId);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"[SubscriptionController] InvalidOperationException: {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SubscriptionController] Exception: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"[SubscriptionController] StackTrace: {ex.StackTrace}");
            return BadRequest(new { error = ex.Message });
        }
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
