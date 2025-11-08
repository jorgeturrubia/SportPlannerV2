using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportPlanner.Application.Dtos.Planning;
using SportPlanner.Application.UseCases.Planning;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;
using System;
using System.Threading.Tasks;

namespace SportPlanner.API.Controllers.Planning;

[ApiController]
[Authorize]
[Route("api/planning/marketplace")]
public class MarketplaceController : ControllerBase
{
    private readonly IMediator _mediator;

    public MarketplaceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Request model for publishing an item
    public record PublishRequest(MarketplaceItemType Type, Guid SourceEntityId);

    // Request model for rating an item
    public record RateRequest(int Stars, string? Comment);

    /// <summary>
    /// Searches the marketplace for items.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<MarketplaceItemDto>), 200)]
    public async Task<IActionResult> Search(
        [FromQuery] Sport sport,
        [FromQuery] MarketplaceItemType? type,
    [FromQuery] MarketplaceFilter filter = MarketplaceFilter.Popular,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var criteria = new MarketplaceSearchDto
        {
            Sport = sport,
            Type = type,
            Filter = filter,
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize
        };

        var query = new SearchMarketplaceQuery(criteria);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets a single marketplace item by its ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MarketplaceItemDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetMarketplaceItemByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Publishes a user's resource to the marketplace.
    /// </summary>
    [HttpPost("publish")]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Publish([FromBody] PublishRequest request)
    {
        var command = new PublishToMarketplaceCommand(request.Type, request.SourceEntityId);
        var newItemId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = newItemId }, newItemId);
    }

    /// <summary>
    /// Downloads (clones) a marketplace item to the user's subscription.
    /// </summary>
    [HttpPost("{id}/download")]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Download(Guid id)
    {
        var command = new DownloadFromMarketplaceCommand(id);
        var clonedEntityId = await _mediator.Send(command);
        return Ok(clonedEntityId);
    }

    /// <summary>
    /// Adds or updates a rating for a marketplace item.
    /// </summary>
    [HttpPost("{id}/rate")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Rate(Guid id, [FromBody] RateRequest request)
    {
        var command = new RateMarketplaceItemCommand(id, request.Stars, request.Comment);
        await _mediator.Send(command);
        return NoContent();
    }
}