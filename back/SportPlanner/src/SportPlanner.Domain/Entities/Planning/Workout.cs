using SportPlanner.Domain.Enum;

namespace SportPlanner.Domain.Entities.Planning;

public class Workout
{
    public Guid Id { get; private set; }
    public Guid? SubscriptionId { get; private set; }
    public ContentOwnership Ownership { get; private set; }
    public Guid? MarketplaceUserId { get; private set; }

    public DateTime Fecha { get; private set; }
    public int? EstimatedDurationMinutes { get; private set; }
    public string? Notes { get; private set; }

    public bool IsActive { get; private set; }

    private readonly List<WorkoutExercise> _exercises = new();
    public IReadOnlyCollection<WorkoutExercise> Exercises => _exercises.AsReadOnly();

    

    // Audit
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    private Workout() { }

    public Workout(
        Guid? subscriptionId,
        ContentOwnership ownership,
        string createdBy,
        DateTime fecha,
        Guid? marketplaceUserId = null)
    {
        ValidateOwnership(subscriptionId, ownership, marketplaceUserId);

        if (fecha == default)
            throw new ArgumentException("Fecha is required", nameof(fecha));

        Id = Guid.NewGuid();
        SubscriptionId = subscriptionId;
        Ownership = ownership;
        MarketplaceUserId = marketplaceUserId;
        Fecha = fecha;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void Update(string updatedBy)
    {
        if (Ownership == ContentOwnership.System)
            throw new InvalidOperationException("Cannot modify system content directly. Clone it first.");

        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void SetMetadata(
        int? estimatedDurationMinutes,
        string? notes,
        string updatedBy)
    {
        if (Ownership == ContentOwnership.System)
            throw new InvalidOperationException("Cannot modify system content directly. Clone it first.");

        if (estimatedDurationMinutes.HasValue && estimatedDurationMinutes.Value <= 0)
            throw new ArgumentException("Duration must be greater than 0", nameof(estimatedDurationMinutes));

        EstimatedDurationMinutes = estimatedDurationMinutes;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void AddExercise(
        Guid exerciseId,
        int order,
        int? sets = null,
        int? reps = null,
        int? durationSeconds = null,
        string? intensity = null,
        int? restSeconds = null,
        string? notes = null)
    {
        if (Ownership == ContentOwnership.System)
            throw new InvalidOperationException("Cannot modify system content directly. Clone it first.");

        if (_exercises.Any(we => we.ExerciseId == exerciseId))
            throw new InvalidOperationException($"Exercise {exerciseId} is already in this workout");

        if (_exercises.Any(we => we.Order == order))
            throw new InvalidOperationException($"Order {order} is already taken");

        var workoutExercise = new WorkoutExercise(
            Id,
            exerciseId,
            order,
            sets,
            reps,
            durationSeconds,
            intensity,
            restSeconds,
            notes);

        _exercises.Add(workoutExercise);
    }

    public void RemoveExercise(Guid exerciseId)
    {
        if (Ownership == ContentOwnership.System)
            throw new InvalidOperationException("Cannot modify system content directly. Clone it first.");

        var exercise = _exercises.FirstOrDefault(we => we.ExerciseId == exerciseId);
        if (exercise == null)
            throw new InvalidOperationException($"Exercise {exerciseId} not found in this workout");

        _exercises.Remove(exercise);
    }

    public void UpdateExercise(
        Guid exerciseId,
        int? sets,
        int? reps,
        int? durationSeconds,
        string? intensity,
        int? restSeconds,
        string? notes)
    {
        if (Ownership == ContentOwnership.System)
            throw new InvalidOperationException("Cannot modify system content directly. Clone it first.");

        var exercise = _exercises.FirstOrDefault(we => we.ExerciseId == exerciseId);
        if (exercise == null)
            throw new InvalidOperationException($"Exercise {exerciseId} not found in this workout");

        exercise.UpdateParameters(sets, reps, durationSeconds, intensity, restSeconds, notes);
    }

    public void ReorderExercises(Dictionary<Guid, int> exerciseOrders)
    {
        if (Ownership == ContentOwnership.System)
            throw new InvalidOperationException("Cannot modify system content directly. Clone it first.");

        foreach (var kvp in exerciseOrders)
        {
            var exercise = _exercises.FirstOrDefault(we => we.ExerciseId == kvp.Key);
            if (exercise == null)
                throw new InvalidOperationException($"Exercise {kvp.Key} not found in this workout");

            exercise.UpdateOrder(kvp.Value);
        }

        // Validate no duplicate orders
        if (_exercises.GroupBy(we => we.Order).Any(g => g.Count() > 1))
            throw new InvalidOperationException("Duplicate orders detected after reordering");
    }

    public void Deactivate(string updatedBy)
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Activate(string updatedBy)
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public Workout Clone(Guid targetSubscriptionId, string createdBy)
    {
        if (Ownership == ContentOwnership.User)
            throw new InvalidOperationException("Cannot clone user content directly.");

        var cloned = new Workout(
            targetSubscriptionId,
            ContentOwnership.User,
            createdBy,
            Fecha
            );

        cloned.EstimatedDurationMinutes = EstimatedDurationMinutes;
        cloned.Notes = Notes;

        // Clone exercises
        foreach (var exercise in _exercises)
        {
            cloned.AddExercise(
                exercise.ExerciseId,
                exercise.Order,
                exercise.Sets,
                exercise.Reps,
                exercise.DurationSeconds,
                exercise.Intensity,
                exercise.RestSeconds,
                exercise.Notes);
        }

        return cloned;
    }

    private static void ValidateOwnership(Guid? subscriptionId, ContentOwnership ownership, Guid? marketplaceUserId)
    {
        if (ownership == ContentOwnership.System && subscriptionId.HasValue)
            throw new ArgumentException("System content cannot have SubscriptionId");

        if (ownership == ContentOwnership.User && !subscriptionId.HasValue)
            throw new ArgumentException("User content must have SubscriptionId");

        if (ownership == ContentOwnership.MarketplaceUser && !marketplaceUserId.HasValue)
            throw new ArgumentException("Marketplace content must have MarketplaceUserId");
    }
}