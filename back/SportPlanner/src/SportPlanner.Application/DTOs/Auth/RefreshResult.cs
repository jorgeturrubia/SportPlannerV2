namespace SportPlanner.Application.DTOs.Auth;

public record RefreshResult(
    string AccessToken,
    string? RefreshToken,
    string? Email,
    System.Collections.Generic.Dictionary<string, object>? UserMetadata
);
