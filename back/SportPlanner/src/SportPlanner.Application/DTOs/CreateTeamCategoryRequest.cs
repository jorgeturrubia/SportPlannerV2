using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs;

public record CreateTeamCategoryRequest(
    string Name,
    string Code,
    string? Description,
    Sport Sport,
    int SortOrder,
    bool? IsActive
);
