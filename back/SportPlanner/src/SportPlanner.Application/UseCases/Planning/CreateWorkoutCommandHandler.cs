using MediatR;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.UseCases.Planning;

public class CreateWorkoutCommandHandler : IRequestHandler<CreateWorkoutCommand, Guid>
{
    private readonly IWorkoutRepository _workoutRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateWorkoutCommandHandler(
        IWorkoutRepository workoutRepository,
        IExerciseRepository exerciseRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _workoutRepository = workoutRepository;
        _exerciseRepository = exerciseRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateWorkoutCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        var dto = request.Workout;

        // Get user's subscription
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User does not have an active subscription");

        // Create workout
        var workout = new Workout(
            subscription.Id,
            ContentOwnership.User,
            userId.ToString(),
            dto.Fecha
           );

        // Set metadata
        if (dto.EstimatedDurationMinutes.HasValue || !string.IsNullOrWhiteSpace(dto.Notes))
        {
            workout.SetMetadata(
                dto.EstimatedDurationMinutes,
                dto.Notes,
                userId.ToString());
        }

        // Add exercises
        foreach (var exerciseDto in dto.Exercises.OrderBy(e => e.Order))
        {
            // Validate exercise exists and is accessible
            var exercise = await _exerciseRepository.GetByIdAsync(exerciseDto.ExerciseId, cancellationToken)
                ?? throw new InvalidOperationException($"Exercise with ID {exerciseDto.ExerciseId} not found");

            // Validate exercise ownership (must be system, user's own, or marketplace)
            if (exercise.SubscriptionId.HasValue && exercise.SubscriptionId != subscription.Id)
            {
                throw new InvalidOperationException($"Exercise {exerciseDto.ExerciseId} is not accessible");
            }

            workout.AddExercise(
                exerciseDto.ExerciseId,
                exerciseDto.Order,
                exerciseDto.Sets,
                exerciseDto.Reps,
                exerciseDto.DurationSeconds,
                exerciseDto.Intensity,
                exerciseDto.RestSeconds,
                exerciseDto.Notes);
        }

        await _workoutRepository.AddAsync(workout, cancellationToken);

        return workout.Id;
    }
}