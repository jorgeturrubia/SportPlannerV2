namespace SportPlanner.Domain.Enum;

/// <summary>
/// Represents the execution status of a workout session.
/// </summary>
public enum WorkoutStatus
{
    /// <summary>
    /// Workout is scheduled but not started yet.
    /// </summary>
    Planned,

    /// <summary>
    /// Workout is currently in progress.
    /// </summary>
    InProgress,

    /// <summary>
    /// Workout has been completed.
    /// </summary>
    Completed,

    /// <summary>
    /// Workout was cancelled and won't be executed.
    /// </summary>
    Cancelled
}