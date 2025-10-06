using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs;

public record CreateTeamRequest(
    string Name,
    TeamColor Color,
    Guid TeamCategoryId,
    Guid GenderId,
    Guid AgeGroupId,
    string? Description = null,
    Guid? CoachSubscriptionUserId = null,
    DateTime? Season = null,
    bool AllowMixedGender = false
);