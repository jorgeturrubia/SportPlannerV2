using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs;

public record TeamCategoryResponse(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    int SortOrder,
    Sport Sport,
    bool IsActive
);