using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs;

public record AgeGroupResponse(
    Guid Id,
    string Name,
    string Code,
    int MinAge,
    int MaxAge,
    Sport Sport,
    int SortOrder,
    bool IsActive
);