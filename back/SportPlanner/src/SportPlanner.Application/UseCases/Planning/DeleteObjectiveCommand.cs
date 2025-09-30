using MediatR;

namespace SportPlanner.Application.UseCases.Planning;

public record DeleteObjectiveCommand(Guid ObjectiveId) : IRequest<Unit>;