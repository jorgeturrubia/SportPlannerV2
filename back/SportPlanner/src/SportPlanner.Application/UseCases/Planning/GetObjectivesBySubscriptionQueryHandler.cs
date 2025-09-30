using MediatR;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

public class GetObjectivesBySubscriptionQueryHandler : IRequestHandler<GetObjectivesBySubscriptionQuery, List<ObjectiveDto>>
{
    private readonly IObjectiveRepository _objectiveRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetObjectivesBySubscriptionQueryHandler(
        IObjectiveRepository objectiveRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _objectiveRepository = objectiveRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<ObjectiveDto>> Handle(GetObjectivesBySubscriptionQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        // Get user's subscription
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User does not have an active subscription");

        // Get objectives
        var objectives = await _objectiveRepository.GetBySubscriptionIdAsync(
            subscription.Id,
            request.IncludeInactive,
            cancellationToken);

        // Map to DTOs
        return objectives.Select(o => new ObjectiveDto
        {
            Id = o.Id,
            SubscriptionId = o.SubscriptionId,
            Ownership = o.Ownership,
            Sport = o.Sport,
            Name = o.Name,
            Description = o.Description,
            ObjectiveCategoryId = o.ObjectiveCategoryId,
            ObjectiveSubcategoryId = o.ObjectiveSubcategoryId,
            IsActive = o.IsActive,
            SourceMarketplaceItemId = o.SourceMarketplaceItemId,
            Techniques = o.Techniques.Select(t => new ObjectiveTechniqueDto
            {
                Description = t.Description,
                Order = t.Order
            }).ToList(),
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt
        }).ToList();
    }
}