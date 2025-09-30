namespace SportPlanner.Application.DTOs.Planning;

public class TrainingPlanDto
{
    public Guid Id { get; set; }
    public Guid SubscriptionId { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TrainingScheduleDto Schedule { get; set; }
    public bool IsActive { get; set; }
    public Guid? MarketplaceItemId { get; set; }
    public List<PlanObjectiveDto> Objectives { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Calculated properties for convenience
    public int DurationDays => (EndDate - StartDate).Days;
    public bool IsTargetSessionsBalanced { get; set; }
}