using MediatR;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.UseCases;

public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, Guid>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscriptionUserRepository? _subscriptionUserRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        ISubscriptionUserRepository? subscriptionUserRepository,
        ICurrentUserService currentUserService)
    {
        _subscriptionRepository = subscriptionRepository;
        _subscriptionUserRepository = subscriptionUserRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var ownerId = _currentUserService.GetUserId();

        // Business rule: One subscription per owner
        if (await _subscriptionRepository.ExistsOwnerIdAsync(ownerId, cancellationToken))
        {
            throw new InvalidOperationException($"A subscription already exists for owner {ownerId}");
        }

        // Create new subscription - domain entity handles limits automatically
        var subscription = new Subscription(ownerId, request.Type, request.Sport);

        await _subscriptionRepository.AddAsync(subscription, cancellationToken);

        // Create relationship between subscription and owner (owner becomes Admin in the subscription)
        if (_subscriptionUserRepository is not null)
        {
            var ownerEmail = _currentUserService.GetUserEmail();
            if (string.IsNullOrWhiteSpace(ownerEmail))
            {
                // fallback to user id string when email is not available (tests/mocks)
                ownerEmail = ownerId.ToString();
            }

            var subscriptionUser = new SubscriptionUser(
                subscription.Id,
                ownerId,
                SportPlanner.Domain.Entities.UserRole.Admin,
                ownerEmail);

            await _subscriptionUserRepository.AddAsync(subscriptionUser, cancellationToken);
        }

        return subscription.Id;
    }
}
