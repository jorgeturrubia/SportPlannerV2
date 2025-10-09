using SportPlanner.Domain.Entities;

namespace SportPlanner.Application.Interfaces;

public interface IAuthService
{
    // Password utilities (kept for legacy compatibility if needed)
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);

    // Supabase Auth integration methods
    Task<Supabase.Gotrue.Session> SignUpAsync(string email, string password);
    Task<Supabase.Gotrue.Session> SignInAsync(string email, string password);
    Task SignOutAsync();

    // Exchange a refresh token for a new session (access + refresh)
    Task<SportPlanner.Application.DTOs.Auth.RefreshResult?> RefreshSessionAsync(string refreshToken);
}
