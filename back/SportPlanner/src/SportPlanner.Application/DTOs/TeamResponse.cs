using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs;

public record TeamResponse(
    Guid Id,
    Guid SubscriptionId,
    string Name,
    TeamColor Color,
    Sport Sport,
    string? Description,
    string? HomeVenue,
    string? CoachName,
    string? ContactEmail,
    string? ContactPhone,
    DateTime? Season,
    int MaxPlayers,
    int CurrentPlayersCount,
    DateTime? LastMatchDate,
    bool AllowMixedGender,
    bool IsActive,
    TeamCategoryResponse Category,
    GenderResponse Gender,
    AgeGroupResponse AgeGroup,
    DateTime CreatedAt
);