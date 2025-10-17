using MediatR;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

public class GetObjectivesBySubscriptionQueryHandler : IRequestHandler<GetObjectivesBySubscriptionQuery, List<ObjectiveDto>>
{
    private readonly IObjectiveRepository _objectiveRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IObjectiveCategoryRepository _categoryRepository;
    private readonly IObjectiveSubcategoryRepository _subcategoryRepository;

    public GetObjectivesBySubscriptionQueryHandler(
        IObjectiveRepository objectiveRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService,
        IObjectiveCategoryRepository categoryRepository,
        IObjectiveSubcategoryRepository subcategoryRepository)
    {
        _objectiveRepository = objectiveRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
        _categoryRepository = categoryRepository;
        _subcategoryRepository = subcategoryRepository;
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

        // Load categories and subcategories to resolve names
        var allCategories = await _categoryRepository.GetAllAsync(cancellationToken);
        var categoryMap = allCategories.ToDictionary(c => c.Id, c => c.Name);

        var allSubcategories = await _subcategoryRepository.GetAllAsync(cancellationToken);
        var subcategoryMap = allSubcategories.ToDictionary(s => s.Id, s => s.Name);

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
            ObjectiveCategoryName = categoryMap.TryGetValue(o.ObjectiveCategoryId, out var cn) ? cn : null,
            ObjectiveSubcategoryName = o.ObjectiveSubcategoryId.HasValue && subcategoryMap.TryGetValue(o.ObjectiveSubcategoryId.Value, out var scn) ? scn : null,
            Level = o.Level,
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