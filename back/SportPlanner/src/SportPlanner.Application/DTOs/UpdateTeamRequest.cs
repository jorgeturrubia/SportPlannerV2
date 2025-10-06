using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs;

public record UpdateTeamRequest(
    string Name,
    TeamColor Color,
    string? Description = null
);
