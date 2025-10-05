namespace SportPlanner.Application.DTOs;

public record UpdateGenderRequest(
    string Name,
    string Code,
    string? Description,
    bool? IsActive
);
