using MediatR;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

public class DeleteCalendarEventCommandHandler : IRequestHandler<DeleteCalendarEventCommand, Unit>
{
    private readonly ICalendarEventRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteCalendarEventCommandHandler(
        ICalendarEventRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteCalendarEventCommand request, CancellationToken cancellationToken)
    {
        var subscriptionId = _currentUserService.GetSubscriptionId();

        var calendarEvent = await _repository.GetByIdAsync(request.Id);

        if (calendarEvent == null)
            throw new InvalidOperationException($"Calendar event {request.Id} not found");

        if (calendarEvent.SubscriptionId != subscriptionId)
            throw new UnauthorizedAccessException("You don't have access to this calendar event");

        _repository.Delete(calendarEvent);
        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}
