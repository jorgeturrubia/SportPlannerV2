using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs;

public record CreateTeamRequest(
    string Name,
    TeamColor Color,
    Guid TeamCategoryId,
    Guid GenderId,
    Guid AgeGroupId,
    string? Description = null,
    string? HomeVenue = null,
    string? CoachName = null,
    string? ContactEmail = null,
    string? ContactPhone = null,
    DateTime? Season = null,
    bool AllowMixedGender = false
);