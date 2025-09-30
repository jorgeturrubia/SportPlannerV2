using MediatR;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

public class DeleteObjectiveCommandHandler : IRequestHandler<DeleteObjectiveCommand, Unit>
{
    private readonly IObjectiveRepository _objectiveRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteObjectiveCommandHandler(
        IObjectiveRepository objectiveRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _objectiveRepository = objectiveRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteObjectiveCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        // Get user's subscription
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User does not have an active subscription");

        // Get objective
        var objective = await _objectiveRepository.GetByIdAsync(request.ObjectiveId, cancellationToken)
            ?? throw new InvalidOperationException($"Objective with ID {request.ObjectiveId} not found");

        // Verify ownership
        if (objective.SubscriptionId != subscription.Id)
        {
            throw new UnauthorizedAccessException("Cannot delete objective from another subscription");
        }

        // Domain will validate if it's editable (User content only)
        // Soft delete by deactivating
        objective.Deactivate();

        await _objectiveRepository.UpdateAsync(objective, cancellationToken);

        return Unit.Value;
    }
}