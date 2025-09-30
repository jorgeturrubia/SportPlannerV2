using MediatR;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

public class CloneObjectiveCommandHandler : IRequestHandler<CloneObjectiveCommand, Guid>
{
    private readonly IObjectiveRepository _objectiveRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public CloneObjectiveCommandHandler(
        IObjectiveRepository objectiveRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _objectiveRepository = objectiveRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CloneObjectiveCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        // Get user's subscription
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User does not have an active subscription");

        // Get source objective
        var sourceObjective = await _objectiveRepository.GetByIdAsync(request.SourceObjectiveId, cancellationToken)
            ?? throw new InvalidOperationException($"Objective with ID {request.SourceObjectiveId} not found");

        // Validate sport matches subscription
        if (sourceObjective.Sport != subscription.Sport)
        {
            throw new InvalidOperationException($"Cannot clone objective from different sport ({sourceObjective.Sport}) to subscription sport ({subscription.Sport})");
        }

        // Clone objective (domain will validate it's System or MarketplaceUser content)
        var clonedObjective = sourceObjective.Clone(subscription.Id);

        await _objectiveRepository.AddAsync(clonedObjective, cancellationToken);

        return clonedObjective.Id;
    }
}