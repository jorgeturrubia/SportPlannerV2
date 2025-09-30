using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs.Planning;

public class ExerciseDto
{
    public Guid Id { get; set; }
    public Guid? SubscriptionId { get; set; }
    public ContentOwnership Ownership { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public Guid TypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public string? ImageUrl { get; set; }
    public string? Instructions { get; set; }
    public int? DefaultSets { get; set; }
    public int? DefaultReps { get; set; }
    public int? DefaultDurationSeconds { get; set; }
    public string? DefaultIntensity { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}