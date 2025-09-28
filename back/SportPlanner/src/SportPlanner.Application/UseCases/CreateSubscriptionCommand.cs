using MediatR;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.UseCases;

public record CreateSubscriptionCommand(SubscriptionType Type, Sport Sport) : IRequest<Guid>;
