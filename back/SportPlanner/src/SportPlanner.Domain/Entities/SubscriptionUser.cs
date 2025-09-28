using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Interfaces;
using System;

namespace SportPlanner.Domain.Entities;

public class SubscriptionUser : Entity, IAuditable
{
    public Guid SubscriptionId { get; private set; }
    public Guid UserId { get; private set; }
    public UserRole RoleInSubscription { get; private set; }
    public string GrantedBy { get; private set; }
    public DateTime GrantedAt { get; private set; }
    public string? RemovedBy { get; private set; }
    public DateTime? RemovedAt { get; private set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // For EF Core
    private SubscriptionUser() { }

    public SubscriptionUser(
        Guid subscriptionId,
        Guid userId,
        UserRole roleInSubscription,
        string grantedBy)
    {
        if (subscriptionId == Guid.Empty)
            throw new ArgumentException("SubscriptionId cannot be empty", nameof(subscriptionId));

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(grantedBy))
            throw new ArgumentException("GrantedBy cannot be empty", nameof(grantedBy));

        SubscriptionId = subscriptionId;
        UserId = userId;
        RoleInSubscription = roleInSubscription;
        GrantedBy = grantedBy;
        GrantedAt = DateTime.UtcNow;

        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateRole(UserRole newRole)
    {
        RoleInSubscription = newRole;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Remove(string removedByEmail)
    {
        if (string.IsNullOrWhiteSpace(removedByEmail))
            throw new ArgumentException("RemovedBy cannot be empty", nameof(removedByEmail));

        RemovedBy = removedByEmail;
        RemovedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsActive => !RemovedAt.HasValue;
}
