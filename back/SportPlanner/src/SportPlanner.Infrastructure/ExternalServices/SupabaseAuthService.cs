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
    private readonly string _supabaseUrl;
    private readonly string _supabaseKey;

    public SupabaseAuthService(IConfiguration configuration)
    {
        _supabaseUrl = configuration["Supabase:Url"] ?? throw new ArgumentNullException("Supabase:Url");
        _supabaseKey = configuration["Supabase:Key"] ?? throw new ArgumentNullException("Supabase:Key");

        _supabaseClient = new Supabase.Client(
            _supabaseUrl,
            _supabaseKey,
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

    public async Task<SportPlanner.Application.DTOs.Auth.RefreshResult?> RefreshSessionAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken)) return null;

        // Call Supabase token endpoint to exchange refresh token
        var supabaseUrl = _supabaseUrl.TrimEnd('/');
        var tokenUrl = $"{supabaseUrl}/auth/v1/token";

        using var http = new System.Net.Http.HttpClient();
        var content = new System.Net.Http.FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string,string>("grant_type", "refresh_token"),
            new KeyValuePair<string,string>("refresh_token", refreshToken)
        });

        // Use service key in header if available
        if (!string.IsNullOrWhiteSpace(_supabaseKey))
        {
            http.DefaultRequestHeaders.Add("apikey", _supabaseKey);
            http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _supabaseKey);
        }

        var res = await http.PostAsync(tokenUrl, content);
        if (!res.IsSuccessStatusCode)
        {
            return null;
        }

        var json = await res.Content.ReadAsStringAsync();
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        var root = doc.RootElement;

        var accessToken = root.GetProperty("access_token").GetString();
        var newRefresh = root.TryGetProperty("refresh_token", out var r) ? r.GetString() : null;
        string? email = null;
        System.Collections.Generic.Dictionary<string, object>? meta = null;

        if (root.TryGetProperty("user", out var userEl))
        {
            if (userEl.TryGetProperty("email", out var e)) email = e.GetString();

            if (userEl.TryGetProperty("user_metadata", out var m) && m.ValueKind == System.Text.Json.JsonValueKind.Object)
            {
                meta = new System.Collections.Generic.Dictionary<string, object>();
                foreach (var prop in m.EnumerateObject())
                {
                    meta[prop.Name] = prop.Value.ToString() ?? string.Empty;
                }
            }
        }

        return new SportPlanner.Application.DTOs.Auth.RefreshResult(accessToken ?? string.Empty, newRefresh, email, meta);
    }
}
