using MediatR;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.UseCases;

public record CreateSubscriptionCommand(SubscriptionType Type, Guid SportId) : IRequest<Guid>;
