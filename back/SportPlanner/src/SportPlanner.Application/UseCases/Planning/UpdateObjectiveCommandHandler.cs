using MediatR;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

public class UpdateObjectiveCommandHandler : IRequestHandler<UpdateObjectiveCommand, Unit>
{
    private readonly IObjectiveRepository _objectiveRepository;
    private readonly IObjectiveCategoryRepository _categoryRepository;
    private readonly IObjectiveSubcategoryRepository _subcategoryRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateObjectiveCommandHandler(
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

    public async Task<Unit> Handle(UpdateObjectiveCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        var dto = request.Objective;

        // Get user's subscription
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User does not have an active subscription");

        // Get objective
        var objective = await _objectiveRepository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Objective with ID {dto.Id} not found");

        // Verify ownership
        if (objective.SubscriptionId != subscription.Id)
        {
            throw new UnauthorizedAccessException("Cannot modify objective from another subscription");
        }

        // Domain will validate if it's editable (User content only)

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

        // Update objective
        objective.Update(
            dto.Name,
            dto.Description,
            dto.ObjectiveCategoryId,
            dto.ObjectiveSubcategoryId);

        // Update techniques
        var techniques = dto.Techniques
            .Select(t => (t.Description, t.Order))
            .ToList();
        objective.UpdateTechniques(techniques);

        await _objectiveRepository.UpdateAsync(objective, cancellationToken);

        return Unit.Value;
    }
}