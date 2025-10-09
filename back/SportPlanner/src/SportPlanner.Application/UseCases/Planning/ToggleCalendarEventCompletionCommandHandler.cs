using MediatR;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

public class ToggleCalendarEventCompletionCommandHandler : IRequestHandler<ToggleCalendarEventCompletionCommand, Unit>
{
    private readonly ICalendarEventRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public ToggleCalendarEventCompletionCommandHandler(
        ICalendarEventRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(ToggleCalendarEventCompletionCommand request, CancellationToken cancellationToken)
    {
        var subscriptionId = _currentUserService.GetSubscriptionId();

        var calendarEvent = await _repository.GetByIdAsync(request.Id);

        if (calendarEvent == null)
            throw new InvalidOperationException($"Calendar event {request.Id} not found");

        if (calendarEvent.SubscriptionId != subscriptionId)
            throw new UnauthorizedAccessException("You don't have access to this calendar event");

        if (calendarEvent.IsCompleted)
            calendarEvent.MarkAsIncomplete();
        else
            calendarEvent.MarkAsCompleted();

        _repository.Update(calendarEvent);
        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}
