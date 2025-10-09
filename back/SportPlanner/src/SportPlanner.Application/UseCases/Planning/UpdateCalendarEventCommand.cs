using MediatR;

namespace SportPlanner.Application.UseCases.Planning;

public record UpdateCalendarEventCommand(
    Guid Id,
    DateTime ScheduledDate,
    int DurationMinutes,
    string? Notes) : IRequest<Unit>;
