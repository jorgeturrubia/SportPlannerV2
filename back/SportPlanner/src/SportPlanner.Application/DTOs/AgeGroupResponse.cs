namespace SportPlanner.Application.DTOs;

public record AgeGroupResponse(
    Guid Id,
    string Name,
    string Code,
    int MinAge,
    int MaxAge,
    Guid SportId,
    int SortOrder,
    bool IsActive
);