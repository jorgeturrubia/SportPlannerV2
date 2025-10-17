using MediatR;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

/// <summary>
/// Handler for adding multiple objectives to a training plan in a single batch transaction
/// </summary>
public class AddMultipleObjectivesToPlanCommandHandler : IRequestHandler<AddMultipleObjectivesToPlanCommand, Unit>
{
    private readonly ITrainingPlanRepository _trainingPlanRepository;
    private readonly IObjectiveRepository _objectiveRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public AddMultipleObjectivesToPlanCommandHandler(
        ITrainingPlanRepository trainingPlanRepository,
        IObjectiveRepository objectiveRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _trainingPlanRepository = trainingPlanRepository;
        _objectiveRepository = objectiveRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(AddMultipleObjectivesToPlanCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        // Get user's subscription
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User does not have an active subscription");

        // Get training plan with its objectives
        var trainingPlan = await _trainingPlanRepository.GetByIdWithObjectivesAsync(request.TrainingPlanId, cancellationToken)
            ?? throw new InvalidOperationException($"Training plan with ID {request.TrainingPlanId} not found");

        // Verify ownership
        if (trainingPlan.SubscriptionId != subscription.Id)
        {
            throw new UnauthorizedAccessException("Cannot modify training plan from another subscription");
        }

        // Validate all objectives exist and are not already in the plan
        foreach (var item in request.Objectives)
        {
            if (!await _objectiveRepository.ExistsAsync(item.ObjectiveId, cancellationToken))
            {
                throw new InvalidOperationException($"Objective with ID {item.ObjectiveId} does not exist");
            }

            // Check if already exists in plan
            if (trainingPlan.HasObjective(item.ObjectiveId))
            {
                throw new InvalidOperationException($"Objective {item.ObjectiveId} is already in this plan");
            }
        }

        // Add all objectives to the plan
        foreach (var item in request.Objectives)
        {
            trainingPlan.AddObjective(item.ObjectiveId, item.Priority, item.TargetSessions);
        }

        // Save the plan with all new objectives (single transaction)
        await _trainingPlanRepository.UpdateAsync(trainingPlan, cancellationToken);

        return Unit.Value;
    }
}
