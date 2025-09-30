namespace SportPlanner.Domain.Enum;

/// <summary>
/// Defines the type of content that can be published in the marketplace.
/// </summary>
public enum MarketplaceItemType
{
    /// <summary>
    /// Training objective/goal.
    /// </summary>
    Objective,

    /// <summary>
    /// Individual exercise.
    /// </summary>
    Exercise,

    /// <summary>
    /// Complete workout session.
    /// </summary>
    Workout,

    /// <summary>
    /// Complete training plan with schedule and objectives.
    /// </summary>
    TrainingPlan
}