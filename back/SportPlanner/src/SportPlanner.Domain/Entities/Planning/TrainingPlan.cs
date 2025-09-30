using SportPlanner.Domain.Interfaces;
using SportPlanner.Domain.ValueObjects;

namespace SportPlanner.Domain.Entities.Planning;

/// <summary>
/// Represents a training plan with objectives distributed over time.
/// Always user-owned (SubscriptionId required).
/// </summary>
public class TrainingPlan : Entity, IAuditable
{
    public Guid SubscriptionId { get; private set; }
    public string Name { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public TrainingSchedule Schedule { get; private set; }
    public bool IsActive { get; private set; }
    public Guid? MarketplaceItemId { get; private set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    private readonly List<PlanObjective> _objectives = new();
    public IReadOnlyCollection<PlanObjective> Objectives => _objectives.AsReadOnly();

    // For EF Core
    private TrainingPlan()
    {
        Name = string.Empty;
        CreatedBy = string.Empty;
        Schedule = null!;
    }

    public TrainingPlan(
        Guid subscriptionId,
        string name,
        DateTime startDate,
        DateTime endDate,
        TrainingSchedule schedule)
    {
        if (subscriptionId == Guid.Empty)
            throw new ArgumentException("SubscriptionId cannot be empty", nameof(subscriptionId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (name.Length > 200)
            throw new ArgumentException("Name cannot exceed 200 characters", nameof(name));

        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date", nameof(endDate));

        if (schedule == null)
            throw new ArgumentNullException(nameof(schedule));

        // Validate that schedule weeks match the plan duration
        var expectedWeeks = (int)Math.Ceiling((endDate - startDate).TotalDays / 7);
        if (schedule.TotalWeeks != expectedWeeks)
            throw new ArgumentException($"Schedule weeks ({schedule.TotalWeeks}) must match plan duration ({expectedWeeks} weeks)", nameof(schedule));

        SubscriptionId = subscriptionId;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        Schedule = schedule;
        IsActive = true;
    }

    public void Update(string name, DateTime startDate, DateTime endDate, TrainingSchedule schedule)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (name.Length > 200)
            throw new ArgumentException("Name cannot exceed 200 characters", nameof(name));

        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date", nameof(endDate));

        if (schedule == null)
            throw new ArgumentNullException(nameof(schedule));

        var expectedWeeks = (int)Math.Ceiling((endDate - startDate).TotalDays / 7);
        if (schedule.TotalWeeks != expectedWeeks)
            throw new ArgumentException($"Schedule weeks must match plan duration", nameof(schedule));

        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        Schedule = schedule;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddObjective(Guid objectiveId, int priority, int targetSessions)
    {
        // Validate objective not already added
        if (_objectives.Any(po => po.ObjectiveId == objectiveId))
            throw new InvalidOperationException($"Objective {objectiveId} is already in this plan");

        // Validate target sessions doesn't exceed total plan sessions
        if (targetSessions > Schedule.TotalSessions)
            throw new ArgumentException($"Target sessions ({targetSessions}) cannot exceed total plan sessions ({Schedule.TotalSessions})", nameof(targetSessions));

        var planObjective = new PlanObjective(Id, objectiveId, priority, targetSessions);
        _objectives.Add(planObjective);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveObjective(Guid objectiveId)
    {
        var planObjective = _objectives.FirstOrDefault(po => po.ObjectiveId == objectiveId);
        if (planObjective == null)
            throw new InvalidOperationException($"Objective {objectiveId} is not in this plan");

        _objectives.Remove(planObjective);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateObjectivePriority(Guid objectiveId, int priority)
    {
        var planObjective = _objectives.FirstOrDefault(po => po.ObjectiveId == objectiveId);
        if (planObjective == null)
            throw new InvalidOperationException($"Objective {objectiveId} is not in this plan");

        planObjective.UpdatePriority(priority);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateObjectiveTargetSessions(Guid objectiveId, int targetSessions)
    {
        var planObjective = _objectives.FirstOrDefault(po => po.ObjectiveId == objectiveId);
        if (planObjective == null)
            throw new InvalidOperationException($"Objective {objectiveId} is not in this plan");

        if (targetSessions > Schedule.TotalSessions)
            throw new ArgumentException($"Target sessions cannot exceed total plan sessions ({Schedule.TotalSessions})", nameof(targetSessions));

        planObjective.UpdateTargetSessions(targetSessions);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets objectives ordered by priority (high to low).
    /// </summary>
    public List<PlanObjective> GetObjectivesByPriority()
    {
        return _objectives.OrderByDescending(po => po.Priority).ToList();
    }

    /// <summary>
    /// Validates that total target sessions for all objectives is reasonable.
    /// </summary>
    public bool IsTargetSessionsBalanced()
    {
        var totalTargetSessions = _objectives.Sum(po => po.TargetSessions);
        // Allow some flexibility: total targets can be 80-120% of actual sessions
        return totalTargetSessions >= Schedule.TotalSessions * 0.8 &&
               totalTargetSessions <= Schedule.TotalSessions * 1.2;
    }
}