namespace SportPlanner.Application.DTOs;

public record GenderResponse(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    bool IsActive
);