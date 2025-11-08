using SportPlanner.Domain.Enum;
using SportPlanner.Domain.Interfaces;
using System;

namespace SportPlanner.Domain.Entities;

public class Subscription : Entity, IAuditable
{
    public Guid OwnerId { get; private set; }
    public SubscriptionType Type { get; private set; }
    public Guid SportId { get; private set; }
    public Sport Sport { get; private set; } = null!;
    public int MaxUsers { get; private set; }
    public int MaxTeams { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // For EF Core
    private Subscription() { }

    public Subscription(Guid ownerId, SubscriptionType type, Guid sportId)
    {
        if (ownerId == Guid.Empty)
            throw new ArgumentException("OwnerId cannot be empty", nameof(ownerId));

        if (sportId == Guid.Empty)
            throw new ArgumentException("SportId cannot be empty", nameof(sportId));

        OwnerId = ownerId;
        Type = type;
        SportId = sportId;
        IsActive = true;

        // Set limits based on type
        SetLimits(type);

        // Domain event could be added here later
        // AddDomainEvent(new SubscriptionCreatedEvent(Id, ownerId, type));
    }

    private void SetLimits(SubscriptionType type)
    {
        switch (type)
        {
            case SubscriptionType.Free:
            case SubscriptionType.Club:
                MaxUsers = 1;
                MaxTeams = 1;
                break;
            case SubscriptionType.Team:
            case SubscriptionType.Coach:
                MaxUsers = 15;
                MaxTeams = 15;
                break;
            default:
                throw new ArgumentException($"Unknown subscription type: {type}", nameof(type));
        }
    }

    public void UpdateSport(Guid newSportId)
    {
        if (newSportId == Guid.Empty)
            throw new ArgumentException("SportId cannot be empty", nameof(newSportId));

        SportId = newSportId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public int GetCurrentUserCount()
    {
        // Owner always counts as 1 user
        // Additional users would be counted from SubscriptionUser collection
        // For now, return 1 (just owner)
        return 1;
    }
}
