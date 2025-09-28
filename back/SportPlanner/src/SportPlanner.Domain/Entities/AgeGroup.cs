using SportPlanner.Domain.Enum;
using SportPlanner.Domain.Interfaces;

namespace SportPlanner.Domain.Entities;

public class AgeGroup : Entity, IAuditable
{
    public string Name { get; private set; }
    public string Code { get; private set; }
    public int MinAge { get; private set; }
    public int MaxAge { get; private set; }
    public Sport Sport { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // For EF Core
    private AgeGroup() { }

    public AgeGroup(string name, string code, int minAge, int maxAge, Sport sport, int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty", nameof(code));

        if (minAge < 0)
            throw new ArgumentException("MinAge cannot be negative", nameof(minAge));

        if (maxAge < minAge)
            throw new ArgumentException("MaxAge cannot be less than MinAge", nameof(maxAge));

        Name = name;
        Code = code.ToUpperInvariant();
        MinAge = minAge;
        MaxAge = maxAge;
        Sport = sport;
        SortOrder = sortOrder;
        IsActive = true;
    }

    public void UpdateDetails(string name, int minAge, int maxAge, int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (minAge < 0)
            throw new ArgumentException("MinAge cannot be negative", nameof(minAge));

        if (maxAge < minAge)
            throw new ArgumentException("MaxAge cannot be less than MinAge", nameof(maxAge));

        Name = name;
        MinAge = minAge;
        MaxAge = maxAge;
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsValidForAge(int age)
    {
        return age >= MinAge && age <= MaxAge;
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