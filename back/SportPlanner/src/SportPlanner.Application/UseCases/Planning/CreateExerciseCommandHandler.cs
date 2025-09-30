using MediatR;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.UseCases.Planning;

public class CreateExerciseCommandHandler : IRequestHandler<CreateExerciseCommand, Guid>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IExerciseCategoryRepository _categoryRepository;
    private readonly IExerciseTypeRepository _typeRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateExerciseCommandHandler(
        IExerciseRepository exerciseRepository,
        IExerciseCategoryRepository categoryRepository,
        IExerciseTypeRepository typeRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _exerciseRepository = exerciseRepository;
        _categoryRepository = categoryRepository;
        _typeRepository = typeRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateExerciseCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        var dto = request.Exercise;

        // Get user's subscription
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User does not have an active subscription");

        // Validate category exists
        if (!await _categoryRepository.ExistsAsync(dto.CategoryId, cancellationToken))
        {
            throw new InvalidOperationException($"Category with ID {dto.CategoryId} does not exist");
        }

        // Validate type exists
        if (!await _typeRepository.ExistsAsync(dto.TypeId, cancellationToken))
        {
            throw new InvalidOperationException($"Type with ID {dto.TypeId} does not exist");
        }

        // Create exercise
        var exercise = new Exercise(
            subscription.Id,
            ContentOwnership.User,
            dto.Name,
            dto.Description,
            dto.CategoryId,
            dto.TypeId,
            userId.ToString());

        // Set optional fields
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

        await _exerciseRepository.AddAsync(exercise, cancellationToken);

        return exercise.Id;
    }
}