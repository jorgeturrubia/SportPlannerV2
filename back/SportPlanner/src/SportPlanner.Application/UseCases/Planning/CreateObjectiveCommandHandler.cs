using MediatR;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Application.UseCases.Planning;

public class CreateObjectiveCommandHandler : IRequestHandler<CreateObjectiveCommand, Guid>
{
    private readonly IObjectiveRepository _objectiveRepository;
    private readonly IObjectiveCategoryRepository _categoryRepository;
    private readonly IObjectiveSubcategoryRepository _subcategoryRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateObjectiveCommandHandler(
        IObjectiveRepository objectiveRepository,
        IObjectiveCategoryRepository categoryRepository,
        IObjectiveSubcategoryRepository subcategoryRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _objectiveRepository = objectiveRepository;
        _categoryRepository = categoryRepository;
        _subcategoryRepository = subcategoryRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateObjectiveCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        var dto = request.Objective;

        // Get user's subscription
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User does not have an active subscription");

        // Validate sport matches subscription
        if (subscription.Sport != dto.Sport)
        {
            throw new InvalidOperationException($"Objective sport ({dto.Sport}) must match subscription sport ({subscription.Sport})");
        }

        // Validate category exists
        if (!await _categoryRepository.ExistsAsync(dto.ObjectiveCategoryId, cancellationToken))
        {
            throw new InvalidOperationException($"Category with ID {dto.ObjectiveCategoryId} does not exist");
        }

        // Validate subcategory if provided
        if (dto.ObjectiveSubcategoryId.HasValue &&
            !await _subcategoryRepository.ExistsAsync(dto.ObjectiveSubcategoryId.Value, cancellationToken))
        {
            throw new InvalidOperationException($"Subcategory with ID {dto.ObjectiveSubcategoryId} does not exist");
        }

        // Create objective
        var objective = new Objective(
            subscription.Id,
            dto.Sport,
            dto.Name,
            dto.Description,
            dto.ObjectiveCategoryId,
            dto.ObjectiveSubcategoryId);

        // Add techniques
        foreach (var technique in dto.Techniques.OrderBy(t => t.Order))
        {
            objective.AddTechnique(technique.Description, technique.Order);
        }

        await _objectiveRepository.AddAsync(objective, cancellationToken);

        return objective.Id;
    }
}