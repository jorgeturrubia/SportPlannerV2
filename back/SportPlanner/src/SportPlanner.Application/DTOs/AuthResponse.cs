namespace SportPlanner.Application.DTOs;

public record AuthResponse(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    string AccessToken,
    string? RefreshToken
);
