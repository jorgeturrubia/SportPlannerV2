using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs;

public record UpdateAgeGroupRequest(
    string Name,
    string Code,
    int MinAge,
    int MaxAge,
    Sport Sport,
    int SortOrder,
    bool? IsActive
);
