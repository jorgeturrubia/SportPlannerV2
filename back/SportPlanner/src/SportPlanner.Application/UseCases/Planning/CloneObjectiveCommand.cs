using MediatR;

namespace SportPlanner.Application.UseCases.Planning;

public record CloneObjectiveCommand(Guid SourceObjectiveId) : IRequest<Guid>;