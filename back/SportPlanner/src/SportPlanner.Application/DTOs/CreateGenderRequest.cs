namespace SportPlanner.Application.DTOs;

public record CreateGenderRequest(
    string Name,
    string Code,
    string? Description,
    bool? IsActive
);
