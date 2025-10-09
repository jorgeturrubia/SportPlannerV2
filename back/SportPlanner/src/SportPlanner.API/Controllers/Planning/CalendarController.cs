using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.UseCases.Planning;

namespace SportPlanner.API.Controllers.Planning;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CalendarController : ControllerBase
{
    private readonly IMediator _mediator;

    public CalendarController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets calendar events with optional filters
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CalendarEventDto>>> GetEvents(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] Guid? teamId = null,
        [FromQuery] bool? isCompleted = null)
    {
        var query = new GetCalendarEventsQuery(startDate, endDate, teamId, isCompleted);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new calendar event
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateEvent([FromBody] CreateCalendarEventDto dto)
    {
        var command = new CreateCalendarEventCommand(
            dto.TeamId,
            dto.WorkoutId,
            dto.TrainingPlanId,
            dto.ScheduledDate,
            dto.DurationMinutes,
            dto.Notes);

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetEvents), new { id }, id);
    }

    /// <summary>
    /// Updates an existing calendar event
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateEvent(Guid id, [FromBody] UpdateCalendarEventDto dto)
    {
        var command = new UpdateCalendarEventCommand(
            id,
            dto.ScheduledDate,
            dto.DurationMinutes,
            dto.Notes);

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Deletes a calendar event
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEvent(Guid id)
    {
        var command = new DeleteCalendarEventCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Toggles the completion status of a calendar event
    /// </summary>
    [HttpPost("{id}/toggle-completion")]
    public async Task<ActionResult> ToggleCompletion(Guid id)
    {
        var command = new ToggleCalendarEventCompletionCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}
