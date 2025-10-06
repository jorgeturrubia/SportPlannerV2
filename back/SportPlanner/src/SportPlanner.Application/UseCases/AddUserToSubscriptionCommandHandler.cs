using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities;

namespace SportPlanner.Application.UseCases;

public class AddUserToSubscriptionCommandHandler : IRequestHandler<AddUserToSubscriptionCommand, Unit>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscriptionUserRepository _subscriptionUserRepository;
    private readonly ICurrentUserService _currentUserService;

    public AddUserToSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        ISubscriptionUserRepository subscriptionUserRepository,
        ICurrentUserService currentUserService)
    {
        _subscriptionRepository = subscriptionRepository;
        _subscriptionUserRepository = subscriptionUserRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(AddUserToSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetUserId();
        var currentUserEmail = _currentUserService.GetUserEmail();
        if (string.IsNullOrWhiteSpace(currentUserEmail))
        {
            currentUserEmail = currentUserId.ToString();
        }

        // Get subscription
        var subscription = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId, cancellationToken);
        if (subscription is null)
        {
            throw new InvalidOperationException($"Subscription {request.SubscriptionId} not found");
        }

        // Verify ownership
        if (subscription.OwnerId != currentUserId)
        {
            throw new UnauthorizedAccessException("Only subscription owner can add users");
        }

        // Check if user is already in subscription
        if (await _subscriptionUserRepository.ExistsUserInSubscriptionAsync(
                request.SubscriptionId,
                request.Request.UserId,
                cancellationToken))
        {
            throw new InvalidOperationException($"User {request.Request.UserId} is already in subscription");
        }

        // Check current users vs limit
        var currentUserCount = await _subscriptionUserRepository
            .GetActiveUserCountBySubscriptionIdAsync(request.SubscriptionId, cancellationToken);

        if (currentUserCount >= subscription.MaxUsers)
        {
            throw new InvalidOperationException($"Subscription has reached maximum users ({subscription.MaxUsers})");
        }

        // Add user to subscription
        var subscriptionUser = new SubscriptionUser(
            request.SubscriptionId,
            request.Request.UserId,
            request.Request.RoleInSubscription,
            currentUserEmail);

        await _subscriptionUserRepository.AddAsync(subscriptionUser, cancellationToken);

        return Unit.Value;
    }
}
