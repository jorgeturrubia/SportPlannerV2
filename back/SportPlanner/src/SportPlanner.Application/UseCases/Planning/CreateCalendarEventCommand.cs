using MediatR;

namespace SportPlanner.Application.UseCases.Planning;

public record CreateCalendarEventCommand(
    Guid TeamId,
    Guid WorkoutId,
    Guid? TrainingPlanId,
    DateTime ScheduledDate,
    int DurationMinutes,
    string? Notes) : IRequest<Guid>;
