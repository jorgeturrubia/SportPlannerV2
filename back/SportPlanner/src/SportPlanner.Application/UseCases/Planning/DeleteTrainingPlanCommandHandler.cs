using MediatR;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

public class DeleteTrainingPlanCommandHandler : IRequestHandler<DeleteTrainingPlanCommand, Unit>
{
    private readonly ITrainingPlanRepository _trainingPlanRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteTrainingPlanCommandHandler(
        ITrainingPlanRepository trainingPlanRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _trainingPlanRepository = trainingPlanRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteTrainingPlanCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        var planId = request.TrainingPlanId;

        Console.WriteLine($"🗑️ DeleteTrainingPlanCommandHandler - Deleting plan ID: {planId}");

        // Get user's subscription
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User does not have an active subscription");

        // Get training plan
        var trainingPlan = await _trainingPlanRepository.GetByIdAsync(planId, cancellationToken)
            ?? throw new InvalidOperationException($"Training plan with ID {planId} not found");

        // Verify ownership
        if (trainingPlan.SubscriptionId != subscription.Id)
        {
            throw new UnauthorizedAccessException("Cannot delete training plan from another subscription");
        }

        // Delete the plan (pass the entity, not just the ID)
        await _trainingPlanRepository.DeleteAsync(trainingPlan, cancellationToken);

        Console.WriteLine($"✅ DeleteTrainingPlanCommandHandler - Plan {planId} deleted successfully");

        return Unit.Value;
    }
}
