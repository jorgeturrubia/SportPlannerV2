using SportPlanner.Domain.Entities;

namespace SportPlanner.Application.DTOs;

public record AddUserToSubscriptionRequest(
    Guid UserId,
    UserRole RoleInSubscription);
