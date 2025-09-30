namespace SportPlanner.Domain.Entities.Planning;

/// <summary>
/// Junction entity representing the many-to-many relationship between TrainingPlan and Objective.
/// Includes additional metadata like priority and target sessions.
/// </summary>
public class PlanObjective
{
    public Guid TrainingPlanId { get; private set; }
    public Guid ObjectiveId { get; private set; }
    public int Priority { get; private set; }
    public int TargetSessions { get; private set; }

    // Navigation properties
    public TrainingPlan TrainingPlan { get; private set; }
    public Objective Objective { get; private set; }

    // For EF Core
    private PlanObjective()
    {
        TrainingPlan = null!;
        Objective = null!;
    }

    public PlanObjective(Guid trainingPlanId, Guid objectiveId, int priority, int targetSessions)
    {
        if (trainingPlanId == Guid.Empty)
            throw new ArgumentException("TrainingPlanId cannot be empty", nameof(trainingPlanId));

        if (objectiveId == Guid.Empty)
            throw new ArgumentException("ObjectiveId cannot be empty", nameof(objectiveId));

        if (priority < 1 || priority > 5)
            throw new ArgumentException("Priority must be between 1 and 5", nameof(priority));

        if (targetSessions < 1)
            throw new ArgumentException("Target sessions must be at least 1", nameof(targetSessions));

        TrainingPlanId = trainingPlanId;
        ObjectiveId = objectiveId;
        Priority = priority;
        TargetSessions = targetSessions;
    }

    public void UpdatePriority(int priority)
    {
        if (priority < 1 || priority > 5)
            throw new ArgumentException("Priority must be between 1 and 5", nameof(priority));

        Priority = priority;
    }

    public void UpdateTargetSessions(int targetSessions)
    {
        if (targetSessions < 1)
            throw new ArgumentException("Target sessions must be at least 1", nameof(targetSessions));

        TargetSessions = targetSessions;
    }
}