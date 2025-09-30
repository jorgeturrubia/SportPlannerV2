using MediatR;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

public class GetWorkoutsBySubscriptionQueryHandler : IRequestHandler<GetWorkoutsBySubscriptionQuery, List<WorkoutDto>>
{
    private readonly IWorkoutRepository _workoutRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetWorkoutsBySubscriptionQueryHandler(
        IWorkoutRepository workoutRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _workoutRepository = workoutRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<WorkoutDto>> Handle(GetWorkoutsBySubscriptionQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        // Get user's subscription
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User does not have an active subscription");

        // Get user's workouts using repository
        var workouts = await _workoutRepository.GetWorkoutsDtoBySubscriptionIdAsync(subscription.Id, cancellationToken);

        return workouts;
    }
}