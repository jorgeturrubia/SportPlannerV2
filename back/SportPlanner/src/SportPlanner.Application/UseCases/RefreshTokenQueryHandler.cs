using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases;

public class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, AuthResponse?>
{
    private readonly IAuthService _authService;

    public RefreshTokenQueryHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponse?> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshSessionAsync(request.RefreshToken);

        if (result is null)
        {
            return null;
        }

        return new AuthResponse(
            Guid.Empty,
            result.UserMetadata?.GetValueOrDefault("first_name")?.ToString() ?? string.Empty,
            result.UserMetadata?.GetValueOrDefault("last_name")?.ToString() ?? string.Empty,
            result.Email ?? string.Empty,
            string.Empty,
            result.AccessToken ?? string.Empty,
            result.RefreshToken
        );
    }
}
