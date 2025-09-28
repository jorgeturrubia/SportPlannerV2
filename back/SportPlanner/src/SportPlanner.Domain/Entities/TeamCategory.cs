using SportPlanner.Domain.Enum;
using SportPlanner.Domain.Interfaces;

namespace SportPlanner.Domain.Entities;

public class TeamCategory : Entity, IAuditable
{
    public string Name { get; private set; }
    public string Code { get; private set; }
    public string? Description { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public Sport Sport { get; private set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // For EF Core
    private TeamCategory() { }

    public TeamCategory(string name, string code, Sport sport, string? description = null, int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty", nameof(code));

        Name = name;
        Code = code.ToUpperInvariant();
        Sport = sport;
        Description = description;
        SortOrder = sortOrder;
        IsActive = true;
    }

    public void UpdateDetails(string name, string? description = null, int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Name = name;
        Description = description;
        SortOrder = sortOrder;
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
}