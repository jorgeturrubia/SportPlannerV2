using Microsoft.Extensions.Configuration;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities;

namespace SportPlanner.Infrastructure.ExternalServices;

/// <summary>
/// Supabase Auth Service - Replaces custom JWT generation with Supabase authentication
/// Follows Authority + Audience pattern as defined in .clinerules/13-supabase-jwt.md
/// </summary>
public class SupabaseAuthService : IAuthService
{
    private readonly Supabase.Client _supabaseClient;

    public SupabaseAuthService(IConfiguration configuration)
    {
        var supabaseUrl = configuration["Supabase:Url"] ?? throw new ArgumentNullException("Supabase:Url");
        var supabaseKey = configuration["Supabase:Key"] ?? throw new ArgumentNullException("Supabase:Key");

        _supabaseClient = new Supabase.Client(
            supabaseUrl,
            supabaseKey,
            new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = false // No need for realtime in backend
            });
    }

    // Password utilities (kept for legacy compatibility if needed)
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    // Supabase Auth integration methods
    public async Task<Supabase.Gotrue.Session> SignUpAsync(string email, string password)
    {
        var result = await _supabaseClient.Auth.SignUp(email, password);
        return result ?? throw new InvalidOperationException("Failed to sign up user");
    }

    public async Task<Supabase.Gotrue.Session> SignInAsync(string email, string password)
    {
        var result = await _supabaseClient.Auth.SignInWithPassword(email, password);
        return result ?? throw new InvalidOperationException("Failed to sign in user");
    }

    public async Task SignOutAsync()
    {
        await _supabaseClient.Auth.SignOut();
    }
}
