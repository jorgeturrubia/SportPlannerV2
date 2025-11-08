using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs;

public record SubscriptionResponse(
    Guid Id,
    Guid OwnerId,
    string OwnerFirstName,
    string OwnerLastName,
    string OwnerEmail,
    SubscriptionType Type,
    Sport Sport,
    int MaxUsers,
    int MaxTeams,
    bool IsActive);
