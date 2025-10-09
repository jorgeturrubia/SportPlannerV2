using MediatR;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

public class UpdateCalendarEventCommandHandler : IRequestHandler<UpdateCalendarEventCommand, Unit>
{
    private readonly ICalendarEventRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCalendarEventCommandHandler(
        ICalendarEventRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(UpdateCalendarEventCommand request, CancellationToken cancellationToken)
    {
        var subscriptionId = _currentUserService.GetSubscriptionId();

        var calendarEvent = await _repository.GetByIdAsync(request.Id);

        if (calendarEvent == null)
            throw new InvalidOperationException($"Calendar event {request.Id} not found");

        if (calendarEvent.SubscriptionId != subscriptionId)
            throw new UnauthorizedAccessException("You don't have access to this calendar event");

        // Check for conflicts (excluding this event)
        var hasConflict = await _repository.HasConflictAsync(
            calendarEvent.TeamId,
            request.ScheduledDate,
            request.DurationMinutes,
            calendarEvent.Id);

        if (hasConflict)
        {
            throw new InvalidOperationException("This time slot conflicts with an existing event for this team");
        }

        calendarEvent.Reschedule(request.ScheduledDate, request.DurationMinutes);
        calendarEvent.UpdateNotes(request.Notes);

        _repository.Update(calendarEvent);
        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}
