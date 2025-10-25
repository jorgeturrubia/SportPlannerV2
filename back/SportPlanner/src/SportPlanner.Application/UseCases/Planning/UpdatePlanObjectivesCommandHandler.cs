using MediatR;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

/// <summary>
/// Handler for replacing/updating ALL objectives for a training plan
/// This synchronizes the objectives collection: removes old ones not in the new list, adds new ones
/// Ensures the plan has EXACTLY the objectives specified in the request
/// </summary>
public class UpdatePlanObjectivesCommandHandler : IRequestHandler<UpdatePlanObjectivesCommand, Unit>
{
    private readonly ITrainingPlanRepository _trainingPlanRepository;
    private readonly IObjectiveRepository _objectiveRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdatePlanObjectivesCommandHandler(
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

    public async Task<Unit> Handle(UpdatePlanObjectivesCommand request, CancellationToken cancellationToken)
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

        // Get the new objective IDs from the request
        var newObjectiveIds = new HashSet<Guid>(request.Objectives.Select(o => o.ObjectiveId));

        // Validate all new objectives exist
        foreach (var objectiveId in newObjectiveIds)
        {
            if (!await _objectiveRepository.ExistsAsync(objectiveId, cancellationToken))
            {
                throw new InvalidOperationException($"Objective with ID {objectiveId} does not exist");
            }
        }

        // Get current objectives in the plan
        var currentObjectiveIds = new HashSet<Guid>(trainingPlan.Objectives.Select(o => o.ObjectiveId));

        // Find objectives to remove (in current but not in new list)
        var objectivesToRemove = currentObjectiveIds.Except(newObjectiveIds).ToList();

        // Find objectives to add (in new list but not in current)
        var objectivesToAdd = newObjectiveIds.Except(currentObjectiveIds).ToList();

        // Remove objectives
        foreach (var objectiveId in objectivesToRemove)
        {
            trainingPlan.RemoveObjective(objectiveId);
        }

        // Add new objectives with their priority and target sessions
        foreach (var item in request.Objectives)
        {
            if (objectivesToAdd.Contains(item.ObjectiveId))
            {
                trainingPlan.AddObjective(item.ObjectiveId, item.Priority, item.TargetSessions);
            }
        }

        // Save the plan with updated objectives (single transaction)
        await _trainingPlanRepository.UpdateAsync(trainingPlan, cancellationToken);

        return Unit.Value;
    }
}
