using MediatR;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

public class GetCalendarEventsQueryHandler : IRequestHandler<GetCalendarEventsQuery, List<CalendarEventDto>>
{
    private readonly ICalendarEventRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public GetCalendarEventsQueryHandler(
        ICalendarEventRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<List<CalendarEventDto>> Handle(GetCalendarEventsQuery request, CancellationToken cancellationToken)
    {
        var subscriptionId = _currentUserService.GetSubscriptionId();

        var events = await _repository.GetBySubscriptionIdAsync(
            subscriptionId,
            request.StartDate,
            request.EndDate,
            request.TeamId,
            request.IsCompleted);

        // TODO: Load team names from Teams repository
        return events.Select(e => new CalendarEventDto
        {
            Id = e.Id,
            SubscriptionId = e.SubscriptionId,
            TeamId = e.TeamId,
            TeamName = "Team Name", // TODO: Get from repository
            WorkoutId = e.WorkoutId,
            TrainingPlanId = e.TrainingPlanId,
            TrainingPlanName = e.TrainingPlan?.Name,
            ScheduledDate = e.ScheduledDate,
            DurationMinutes = e.DurationMinutes,
            Notes = e.Notes,
            IsCompleted = e.IsCompleted,
            CompletedAt = e.CompletedAt,
            CreatedAt = e.CreatedAt
        }).ToList();
    }
}
