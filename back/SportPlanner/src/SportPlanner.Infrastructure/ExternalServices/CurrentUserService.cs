using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SportPlanner.Application.Interfaces;
using System;

namespace SportPlanner.Infrastructure.ExternalServices;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;

    // Cache for current request to avoid multiple database lookups
    private Guid? _cachedUserId;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }

    public Guid GetUserId()
    {
        // Return cached value if available (per-request cache)
        if (_cachedUserId.HasValue)
        {
            return _cachedUserId.Value;
        }

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            Console.WriteLine("[CurrentUserService] ERROR: HTTP context is not available");
            throw new UnauthorizedAccessException("HTTP context is not available");
        }

        // Extract Supabase User ID from JWT "sub" claim
        var supabaseUserId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? httpContext.User.FindFirst("sub")?.Value;

        Console.WriteLine($"[CurrentUserService] Supabase User ID from JWT: {supabaseUserId ?? "NULL"}");
        Console.WriteLine($"[CurrentUserService] IsAuthenticated: {httpContext.User.Identity?.IsAuthenticated}");
        Console.WriteLine($"[CurrentUserService] Claims count: {httpContext.User.Claims.Count()}");
        
        // Log all claims for debugging
        foreach (var claim in httpContext.User.Claims)
        {
            Console.WriteLine($"[CurrentUserService] Claim: {claim.Type} = {claim.Value}");
        }

        if (string.IsNullOrEmpty(supabaseUserId))
        {
            Console.WriteLine("[CurrentUserService] ERROR: No 'sub' claim found in JWT");
            throw new UnauthorizedAccessException("User is not authenticated - no 'sub' claim found in JWT");
        }

        // Look up user in database by Supabase User ID (synchronous call - acceptable for scoped service)
        Console.WriteLine($"[CurrentUserService] Looking up user in database by SupabaseUserId: {supabaseUserId}");
        var user = _userRepository.GetBySupabaseUserIdAsync(supabaseUserId).GetAwaiter().GetResult();

        if (user == null)
        {
            Console.WriteLine($"[CurrentUserService] ERROR: User with Supabase ID '{supabaseUserId}' not found in database");
            throw new UnauthorizedAccessException($"User with Supabase ID '{supabaseUserId}' not found in database");
        }

        Console.WriteLine($"[CurrentUserService] User found in database: Id={user.Id}, Email={user.Email.Value}");

        // Cache for this request
        _cachedUserId = user.Id;

        return user.Id;
    }

    public string GetUserEmail()
    {
        var emailClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value
            ?? _httpContextAccessor.HttpContext?.User.FindFirst("email")?.Value;

        return emailClaim ?? throw new UnauthorizedAccessException("Email not found in token");
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;
}
