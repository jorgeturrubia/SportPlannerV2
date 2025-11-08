namespace SportPlanner.Application.DTOs;

public record TeamCategoryResponse(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    int SortOrder,
    Guid SportId,
    bool IsActive
);