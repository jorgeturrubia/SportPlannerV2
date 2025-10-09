using MediatR;
using SportPlanner.Application.DTOs.Planning;

namespace SportPlanner.Application.UseCases.Planning;

public record GetCalendarEventsQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    Guid? TeamId = null,
    bool? IsCompleted = null) : IRequest<List<CalendarEventDto>>;
