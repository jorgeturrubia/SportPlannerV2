using MediatR;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.UseCases.Planning;

public class CloneExerciseCommandHandler : IRequestHandler<CloneExerciseCommand, Guid>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public CloneExerciseCommandHandler(
        IExerciseRepository exerciseRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _exerciseRepository = exerciseRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CloneExerciseCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        // Get user's subscription
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User does not have an active subscription");

        // Get source exercise
        var sourceExercise = await _exerciseRepository.GetByIdAsync(request.ExerciseId, cancellationToken)
            ?? throw new InvalidOperationException($"Exercise with ID {request.ExerciseId} not found");

        // Validate source is System or MarketplaceUser content
        if (sourceExercise.Ownership == ContentOwnership.User)
        {
            throw new InvalidOperationException("Cannot clone user content. Only system or marketplace content can be cloned.");
        }

        // Clone exercise
        var clonedExercise = sourceExercise.Clone(subscription.Id, userId.ToString());

        await _exerciseRepository.AddAsync(clonedExercise, cancellationToken);

        return clonedExercise.Id;
    }
}