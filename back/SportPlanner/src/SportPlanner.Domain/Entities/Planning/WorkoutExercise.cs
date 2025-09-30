namespace SportPlanner.Domain.Entities.Planning;

public class WorkoutExercise
{
    public Guid WorkoutId { get; private set; }
    public Guid ExerciseId { get; private set; }
    public int Order { get; private set; }

    public int? Sets { get; private set; }
    public int? Reps { get; private set; }
    public int? DurationSeconds { get; private set; }
    public string? Intensity { get; private set; }
    public int? RestSeconds { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties
    public Workout Workout { get; private set; } = null!;
    public Exercise Exercise { get; private set; } = null!;

    private WorkoutExercise() { }

    public WorkoutExercise(
        Guid workoutId,
        Guid exerciseId,
        int order,
        int? sets = null,
        int? reps = null,
        int? durationSeconds = null,
        string? intensity = null,
        int? restSeconds = null,
        string? notes = null)
    {
        if (order <= 0)
            throw new ArgumentException("Order must be greater than 0", nameof(order));

        if (sets.HasValue && sets.Value <= 0)
            throw new ArgumentException("Sets must be greater than 0", nameof(sets));

        if (reps.HasValue && reps.Value <= 0)
            throw new ArgumentException("Reps must be greater than 0", nameof(reps));

        if (durationSeconds.HasValue && durationSeconds.Value <= 0)
            throw new ArgumentException("Duration must be greater than 0", nameof(durationSeconds));

        if (restSeconds.HasValue && restSeconds.Value < 0)
            throw new ArgumentException("Rest cannot be negative", nameof(restSeconds));

        WorkoutId = workoutId;
        ExerciseId = exerciseId;
        Order = order;
        Sets = sets;
        Reps = reps;
        DurationSeconds = durationSeconds;
        Intensity = intensity;
        RestSeconds = restSeconds;
        Notes = notes;
    }

    public void UpdateParameters(
        int? sets,
        int? reps,
        int? durationSeconds,
        string? intensity,
        int? restSeconds,
        string? notes)
    {
        if (sets.HasValue && sets.Value <= 0)
            throw new ArgumentException("Sets must be greater than 0", nameof(sets));

        if (reps.HasValue && reps.Value <= 0)
            throw new ArgumentException("Reps must be greater than 0", nameof(reps));

        if (durationSeconds.HasValue && durationSeconds.Value <= 0)
            throw new ArgumentException("Duration must be greater than 0", nameof(durationSeconds));

        if (restSeconds.HasValue && restSeconds.Value < 0)
            throw new ArgumentException("Rest cannot be negative", nameof(restSeconds));

        Sets = sets;
        Reps = reps;
        DurationSeconds = durationSeconds;
        Intensity = intensity;
        RestSeconds = restSeconds;
        Notes = notes;
    }

    public void UpdateOrder(int newOrder)
    {
        if (newOrder <= 0)
            throw new ArgumentException("Order must be greater than 0", nameof(newOrder));

        Order = newOrder;
    }
}