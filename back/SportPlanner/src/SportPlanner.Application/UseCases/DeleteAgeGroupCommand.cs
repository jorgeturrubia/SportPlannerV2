using MediatR;

namespace SportPlanner.Application.UseCases;

public record DeleteAgeGroupCommand(Guid Id) : IRequest<bool>;
