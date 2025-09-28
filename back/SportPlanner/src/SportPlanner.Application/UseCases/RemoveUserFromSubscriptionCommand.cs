using MediatR;

namespace SportPlanner.Application.UseCases;

public record RemoveUserFromSubscriptionCommand(
    Guid SubscriptionId,
    Guid UserIdToRemove) : IRequest<Unit>;
