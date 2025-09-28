using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases;

public class GetUserSubscriptionQueryHandler : IRequestHandler<GetUserSubscriptionQuery, SubscriptionResponse?>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetUserSubscriptionQueryHandler(
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<SubscriptionResponse?> Handle(GetUserSubscriptionQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken);

        return subscription is null ? null : new SubscriptionResponse(
            subscription.Id,
            subscription.OwnerId,
            subscription.Type,
            subscription.Sport,
            subscription.MaxUsers,
            subscription.MaxTeams,
            subscription.IsActive);
    }
}
