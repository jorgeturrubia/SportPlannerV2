namespace SportPlanner.Application.DTOs;

public record LoginUserRequest(
    string Email,
    string Password
);
