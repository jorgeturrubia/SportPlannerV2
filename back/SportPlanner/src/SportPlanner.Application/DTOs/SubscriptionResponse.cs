using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs;

public record SubscriptionResponse(
    Guid Id,
    Guid OwnerId,
    SubscriptionType Type,
    Sport Sport,
    int MaxUsers,
    int MaxTeams,
    bool IsActive);
