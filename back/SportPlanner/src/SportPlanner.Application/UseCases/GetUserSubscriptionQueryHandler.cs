using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases;

public class GetUserSubscriptionQueryHandler : IRequestHandler<GetUserSubscriptionQuery, SubscriptionResponse?>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetUserSubscriptionQueryHandler(
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<SubscriptionResponse?> Handle(GetUserSubscriptionQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken);

        if (subscription is null)
            return null;

        // Enrich with owner data
        var owner = await _userRepository.GetByIdAsync(subscription.OwnerId, cancellationToken);
        if (owner is null)
            throw new InvalidOperationException("Subscription owner not found");

        return new SubscriptionResponse(
            subscription.Id,
            subscription.OwnerId,
            owner.FirstName,
            owner.LastName,
            owner.Email.Value,
            subscription.Type,
            subscription.Sport,
            subscription.MaxUsers,
            subscription.MaxTeams,
            subscription.IsActive);
    }
}
