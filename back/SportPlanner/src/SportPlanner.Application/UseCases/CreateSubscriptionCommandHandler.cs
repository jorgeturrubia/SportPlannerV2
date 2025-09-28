using MediatR;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.UseCases;

public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, Guid>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _subscriptionRepository = subscriptionRepository;
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

        return subscription.Id;
    }
}
