using MediatR;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

public class GetExercisesBySubscriptionQueryHandler : IRequestHandler<GetExercisesBySubscriptionQuery, List<ExerciseDto>>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetExercisesBySubscriptionQueryHandler(
        IExerciseRepository exerciseRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _exerciseRepository = exerciseRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<ExerciseDto>> Handle(GetExercisesBySubscriptionQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        // Get user's subscription
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User does not have an active subscription");

        // Get user's exercises using repository
        var exercises = await _exerciseRepository.GetExercisesDtoBySubscriptionIdAsync(subscription.Id, cancellationToken);

        return exercises;
    }
}