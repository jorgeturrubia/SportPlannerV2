using MediatR;

namespace SportPlanner.Application.UseCases.Planning;

public record GenerateWorkoutsFromPlanCommand(Guid TrainingPlanId) : IRequest<List<Guid>>;