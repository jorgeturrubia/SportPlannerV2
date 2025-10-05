using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs;

public record UpdateTeamCategoryRequest(
    string Name,
    string Code,
    string? Description,
    Sport Sport,
    int SortOrder,
    bool? IsActive
);
