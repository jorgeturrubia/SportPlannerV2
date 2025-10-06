using MediatR;

namespace SportPlanner.Application.UseCases;

public record DeleteTeamCommand(Guid SubscriptionId, Guid TeamId) : IRequest<bool>;
