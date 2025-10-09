using SportPlanner.Domain.Interfaces;

namespace SportPlanner.Domain.Entities.Planning;

/// <summary>
/// Represents a scheduled workout for a specific team on a specific date/time.
/// Always user-owned (SubscriptionId required).
/// </summary>
public class CalendarEvent : Entity, IAuditable
{
    public Guid SubscriptionId { get; private set; }
    public Guid TeamId { get; private set; }
    public Guid WorkoutId { get; private set; }
    public Guid? TrainingPlanId { get; private set; }

    public DateTime ScheduledDate { get; private set; }
    public int DurationMinutes { get; private set; }

    public string? Notes { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public Workout? Workout { get; private set; }
    public TrainingPlan? TrainingPlan { get; private set; }

    // For EF Core
    private CalendarEvent()
    {
        CreatedBy = string.Empty;
    }

    public CalendarEvent(
        Guid subscriptionId,
        Guid teamId,
        Guid workoutId,
        DateTime scheduledDate,
        int durationMinutes,
        Guid? trainingPlanId = null,
        string? notes = null)
    {
        if (subscriptionId == Guid.Empty)
            throw new ArgumentException("SubscriptionId cannot be empty", nameof(subscriptionId));

        if (teamId == Guid.Empty)
            throw new ArgumentException("TeamId cannot be empty", nameof(teamId));

        if (workoutId == Guid.Empty)
            throw new ArgumentException("WorkoutId cannot be empty", nameof(workoutId));

        if (durationMinutes <= 0)
            throw new ArgumentException("Duration must be greater than 0", nameof(durationMinutes));

        if (durationMinutes > 480) // Max 8 hours
            throw new ArgumentException("Duration cannot exceed 480 minutes (8 hours)", nameof(durationMinutes));

        SubscriptionId = subscriptionId;
        TeamId = teamId;
        WorkoutId = workoutId;
        TrainingPlanId = trainingPlanId;
        ScheduledDate = scheduledDate;
        DurationMinutes = durationMinutes;
        Notes = notes;
        IsCompleted = false;
    }

    public void Reschedule(DateTime newScheduledDate, int newDurationMinutes)
    {
        if (IsCompleted)
            throw new InvalidOperationException("Cannot reschedule a completed event");

        if (newDurationMinutes <= 0)
            throw new ArgumentException("Duration must be greater than 0", nameof(newDurationMinutes));

        if (newDurationMinutes > 480)
            throw new ArgumentException("Duration cannot exceed 480 minutes (8 hours)", nameof(newDurationMinutes));

        ScheduledDate = newScheduledDate;
        DurationMinutes = newDurationMinutes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateWorkout(Guid newWorkoutId)
    {
        if (IsCompleted)
            throw new InvalidOperationException("Cannot change workout for a completed event");

        if (newWorkoutId == Guid.Empty)
            throw new ArgumentException("WorkoutId cannot be empty", nameof(newWorkoutId));

        WorkoutId = newWorkoutId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsCompleted()
    {
        if (IsCompleted)
            throw new InvalidOperationException("Event is already marked as completed");

        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsIncomplete()
    {
        if (!IsCompleted)
            throw new InvalidOperationException("Event is not marked as completed");

        IsCompleted = false;
        CompletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if this event is in the past.
    /// </summary>
    public bool IsPastEvent()
    {
        return ScheduledDate.Date < DateTime.UtcNow.Date;
    }

    /// <summary>
    /// Checks if this event is today.
    /// </summary>
    public bool IsToday()
    {
        return ScheduledDate.Date == DateTime.UtcNow.Date;
    }

    /// <summary>
    /// Checks if this event conflicts with another event (overlapping time).
    /// </summary>
    public bool ConflictsWith(CalendarEvent other)
    {
        var thisEnd = ScheduledDate.AddMinutes(DurationMinutes);
        var otherEnd = other.ScheduledDate.AddMinutes(other.DurationMinutes);

        return ScheduledDate < otherEnd && thisEnd > other.ScheduledDate;
    }
}
