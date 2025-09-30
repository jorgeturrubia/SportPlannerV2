using MediatR;

namespace SportPlanner.Application.UseCases.Planning;

public record AddObjectiveToPlanCommand(
    Guid TrainingPlanId,
    Guid ObjectiveId,
    int Priority,
    int TargetSessions) : IRequest<Unit>;