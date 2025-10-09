using MediatR;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Application.UseCases.Planning;

public class CreateCalendarEventCommandHandler : IRequestHandler<CreateCalendarEventCommand, Guid>
{
    private readonly ICalendarEventRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public CreateCalendarEventCommandHandler(
        ICalendarEventRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateCalendarEventCommand request, CancellationToken cancellationToken)
    {
        var subscriptionId = _currentUserService.GetSubscriptionId();

        // Check for conflicts
        var hasConflict = await _repository.HasConflictAsync(
            request.TeamId,
            request.ScheduledDate,
            request.DurationMinutes);

        if (hasConflict)
        {
            throw new InvalidOperationException("This time slot conflicts with an existing event for this team");
        }

        var calendarEvent = new CalendarEvent(
            subscriptionId,
            request.TeamId,
            request.WorkoutId,
            request.ScheduledDate,
            request.DurationMinutes,
            request.TrainingPlanId,
            request.Notes);

        await _repository.AddAsync(calendarEvent);
        await _repository.SaveChangesAsync();

        return calendarEvent.Id;
    }
}
