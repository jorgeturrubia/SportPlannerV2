using MediatR;

namespace SportPlanner.Application.UseCases.Planning;

public record DeleteTrainingPlanCommand(Guid TrainingPlanId) : IRequest<Unit>;
