using MediatR;

namespace SportPlanner.Application.UseCases;

public record LogoutCommand() : IRequest<Unit>;
