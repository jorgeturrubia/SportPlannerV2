using MediatR;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities;

namespace SportPlanner.Application.UseCases;

public class RemoveUserFromSubscriptionCommandHandler : IRequestHandler<RemoveUserFromSubscriptionCommand, Unit>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscriptionUserRepository _subscriptionUserRepository;
    private readonly ICurrentUserService _currentUserService;

    public RemoveUserFromSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        ISubscriptionUserRepository subscriptionUserRepository,
        ICurrentUserService currentUserService)
    {
        _subscriptionRepository = subscriptionRepository;
        _subscriptionUserRepository = subscriptionUserRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(RemoveUserFromSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetUserId();
        var currentUserEmail = _currentUserService.GetUserEmail();

        // Get subscription
        var subscription = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId, cancellationToken);
        if (subscription is null)
        {
            throw new InvalidOperationException($"Subscription {request.SubscriptionId} not found");
        }

        // Verify ownership
        if (subscription.OwnerId != currentUserId)
        {
            throw new UnauthorizedAccessException("Only subscription owner can remove users");
        }

        // Get active subscription user
        var subscriptionUser = await _subscriptionUserRepository
            .GetBySubscriptionAndUserIdAsync(request.SubscriptionId, request.UserIdToRemove, cancellationToken);

        if (subscriptionUser is null || subscriptionUser.RemovedAt.HasValue)
        {
            throw new InvalidOperationException($"User {request.UserIdToRemove} is not an active member of this subscription");
        }

        // Remove user (soft delete with audit)
        subscriptionUser.Remove(currentUserEmail);

        await _subscriptionUserRepository.UpdateAsync(subscriptionUser, cancellationToken);

        return Unit.Value;
    }
}
