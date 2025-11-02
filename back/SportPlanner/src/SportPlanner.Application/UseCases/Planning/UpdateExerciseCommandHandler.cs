using MediatR;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

public class UpdateExerciseCommandHandler : IRequestHandler<UpdateExerciseCommand, Unit>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateExerciseCommandHandler(
        IExerciseRepository exerciseRepository,
        ICurrentUserService currentUserService)
    {
        _exerciseRepository = exerciseRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(UpdateExerciseCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        var exercise = await _exerciseRepository.GetByIdAsync(request.ExerciseId, cancellationToken)
            ?? throw new InvalidOperationException($"Exercise with ID {request.ExerciseId} not found");

        // Check ownership - only allow updating user-owned exercises
        if (exercise.SubscriptionId != _currentUserService.GetSubscriptionId())
            throw new InvalidOperationException("You can only update your own exercises");

        var dto = request.Exercise;

        // Update basic fields
        exercise.Update(
            dto.Name,
            dto.Description,           
            userId.ToString(),
            dto.AnimationJson);

        // Update optional fields
        if (!string.IsNullOrWhiteSpace(dto.VideoUrl) || !string.IsNullOrWhiteSpace(dto.ImageUrl))
        {
            exercise.SetMediaUrls(dto.VideoUrl, dto.ImageUrl, userId.ToString());
        }

        if (!string.IsNullOrWhiteSpace(dto.Instructions))
        {
            exercise.SetInstructions(dto.Instructions, userId.ToString());
        }

        if (dto.DefaultSets.HasValue || dto.DefaultReps.HasValue || dto.DefaultDurationSeconds.HasValue)
        {
            exercise.SetDefaults(
                dto.DefaultSets,
                dto.DefaultReps,
                dto.DefaultDurationSeconds,
                dto.DefaultIntensity,
                userId.ToString());
        }

        // Update exercise-objective relationships
        await _exerciseRepository.UpdateExerciseObjectivesAsync(exercise.Id, dto.ObjectiveIds, cancellationToken);

        await _exerciseRepository.UpdateAsync(exercise, cancellationToken);

        return Unit.Value;
    }
}