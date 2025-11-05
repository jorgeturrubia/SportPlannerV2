using MediatR;
using SportPlanner.Application.Interfaces;
using SportPlanner.Shared.Exceptions;

namespace SportPlanner.Application.UseCases.Planning;

/// <summary>
/// Handles deletion of a workout
/// </summary>
public sealed class DeleteWorkoutCommandHandler : IRequestHandler<DeleteWorkoutCommand, Unit>
{
    private readonly IWorkoutRepository _workoutRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteWorkoutCommandHandler(
        IWorkoutRepository workoutRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _workoutRepository = workoutRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteWorkoutCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        // Get user's subscription
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User does not have an active subscription");

        var workout = await _workoutRepository.GetByIdAsync(request.WorkoutId, cancellationToken);

        if (workout is null)
        {
            throw new NotFoundException($"Workout with ID {request.WorkoutId} not found.");
        }

        // Validate that the workout belongs to the subscription
        if (workout.SubscriptionId != subscription.Id)
        {
            throw new ForbiddenException($"You do not have permission to delete this workout.");
        }

        await _workoutRepository.DeleteAsync(workout.Id, cancellationToken);

        return Unit.Value;
    }
}
