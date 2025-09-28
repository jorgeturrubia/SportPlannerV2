using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities;

namespace SportPlanner.Application.UseCases;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponse?>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public RegisterUserCommandHandler(IUserRepository userRepository, IAuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<AuthResponse?> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Create user in Supabase Auth
        var session = await _authService.SignUpAsync(request.Email, request.Password);

        if (session?.User is null)
        {
            return null;
        }

        // Create local user record for data tracking
        var email = SportPlanner.Domain.ValueObjects.Email.Create(request.Email);

        // Store user data with Supabase user metadata
        var user = new User(request.FirstName, request.LastName, email, "", UserRole.Admin);

        // Save user to local database
        await _userRepository.AddAsync(user, cancellationToken);

        // Note: With Supabase email confirmation flow, the session might not have an access token yet
        // The user needs to confirm email before they can sign in
        return new AuthResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email.Value,
            user.Role.ToString(),
            session.AccessToken // Will be null if email confirmation is required
        );
    }
}
