namespace SportPlanner.Application.DTOs.Planning;

public class CalendarEventDto
{
    public Guid Id { get; set; }
    public Guid SubscriptionId { get; set; }
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public Guid WorkoutId { get; set; }
    public string WorkoutName { get; set; } = string.Empty;
    public Guid? TrainingPlanId { get; set; }
    public string? TrainingPlanName { get; set; }
    public DateTime ScheduledDate { get; set; }
    public int DurationMinutes { get; set; }
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCalendarEventDto
{
    public Guid TeamId { get; set; }
    public Guid WorkoutId { get; set; }
    public Guid? TrainingPlanId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public int DurationMinutes { get; set; }
    public string? Notes { get; set; }
}

public class UpdateCalendarEventDto
{
    public DateTime ScheduledDate { get; set; }
    public int DurationMinutes { get; set; }
    public string? Notes { get; set; }
}
