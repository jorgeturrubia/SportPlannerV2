namespace SportPlanner.Domain.Entities.Planning;

/// <summary>
/// Junction entity representing the many-to-many relationship between Exercise and Objective.
/// Indicates which objectives an exercise helps achieve.
/// </summary>
public class ExerciseObjective
{
    public Guid ExerciseId { get; private set; }
    public Guid ObjectiveId { get; private set; }

    // Navigation properties
    public Exercise Exercise { get; private set; } = null!;
    public Objective Objective { get; private set; } = null!;

    // For EF Core
    private ExerciseObjective() { }

    public ExerciseObjective(Guid exerciseId, Guid objectiveId)
    {
        if (exerciseId == Guid.Empty)
            throw new ArgumentException("ExerciseId cannot be empty", nameof(exerciseId));

        if (objectiveId == Guid.Empty)
            throw new ArgumentException("ObjectiveId cannot be empty", nameof(objectiveId));

        ExerciseId = exerciseId;
        ObjectiveId = objectiveId;
    }
}