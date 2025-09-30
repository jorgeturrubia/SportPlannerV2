namespace SportPlanner.Domain.Entities.Planning;

/// <summary>
/// Junction entity representing the many-to-many relationship between Workout and Objective.
/// Indicates which objectives a workout is designed to achieve.
/// </summary>
public class WorkoutObjective
{
    public Guid WorkoutId { get; private set; }
    public Guid ObjectiveId { get; private set; }

    // Navigation properties
    public Workout Workout { get; private set; } = null!;
    public Objective Objective { get; private set; } = null!;

    // For EF Core
    private WorkoutObjective() { }

    public WorkoutObjective(Guid workoutId, Guid objectiveId)
    {
        if (workoutId == Guid.Empty)
            throw new ArgumentException("WorkoutId cannot be empty", nameof(workoutId));

        if (objectiveId == Guid.Empty)
            throw new ArgumentException("ObjectiveId cannot be empty", nameof(objectiveId));

        WorkoutId = workoutId;
        ObjectiveId = objectiveId;
    }
}