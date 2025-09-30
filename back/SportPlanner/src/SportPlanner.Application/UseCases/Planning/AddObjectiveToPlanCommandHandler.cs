using MediatR;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

public class AddObjectiveToPlanCommandHandler : IRequestHandler<AddObjectiveToPlanCommand, Unit>
{
    private readonly ITrainingPlanRepository _trainingPlanRepository;
    private readonly IObjectiveRepository _objectiveRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public AddObjectiveToPlanCommandHandler(
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

    public async Task<Unit> Handle(AddObjectiveToPlanCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        // Get user's subscription
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User does not have an active subscription");

        // Get training plan
        var trainingPlan = await _trainingPlanRepository.GetByIdWithObjectivesAsync(request.TrainingPlanId, cancellationToken)
            ?? throw new InvalidOperationException($"Training plan with ID {request.TrainingPlanId} not found");

        // Verify ownership
        if (trainingPlan.SubscriptionId != subscription.Id)
        {
            throw new UnauthorizedAccessException("Cannot modify training plan from another subscription");
        }

        // Validate objective exists
        if (!await _objectiveRepository.ExistsAsync(request.ObjectiveId, cancellationToken))
        {
            throw new InvalidOperationException($"Objective with ID {request.ObjectiveId} does not exist");
        }

        // Add objective
        trainingPlan.AddObjective(request.ObjectiveId, request.Priority, request.TargetSessions);

        await _trainingPlanRepository.UpdateAsync(trainingPlan, cancellationToken);

        return Unit.Value;
    }
}