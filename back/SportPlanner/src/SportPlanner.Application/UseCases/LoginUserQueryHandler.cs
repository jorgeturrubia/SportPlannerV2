using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.ValueObjects;

namespace SportPlanner.Application.UseCases;

public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, AuthResponse?>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public LoginUserQueryHandler(IUserRepository userRepository, IAuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<AuthResponse?> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        // Get user by email
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return null;
        }

        // Verify password
        if (!_authService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return null;
        }

        // Generate JWT token
        var token = _authService.GenerateJwtToken(user);

        return new AuthResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email.Value,
            user.Role.ToString(),
            token
        );
    }
}
