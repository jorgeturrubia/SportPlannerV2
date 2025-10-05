using MediatR;

namespace SportPlanner.Application.UseCases;

public record DeleteTeamCategoryCommand(Guid Id) : IRequest<bool>;
