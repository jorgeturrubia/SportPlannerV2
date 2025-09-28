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
        try
        {
            // Authenticate with Supabase
            var session = await _authService.SignInAsync(request.Email, request.Password);

            if (session?.User is null)
            {
                return null;
            }

            // Sync user data with local database
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user is null)
            {
                // First time login - create local user record
                var email = Email.Create(request.Email);
                var supabaseUser = session.User;

                // Extract name from metadata if available, otherwise use email prefix
                var firstName = supabaseUser.UserMetadata?.TryGetValue("first_name", out var fName) == true ? fName?.ToString() : "User";
                var lastName = supabaseUser.UserMetadata?.TryGetValue("last_name", out var lName) == true ? lName?.ToString() : supabaseUser.Email?.Split('@')[0] ?? "Unknown";

                user = new SportPlanner.Domain.Entities.User(firstName, lastName, email, "", SportPlanner.Domain.Entities.UserRole.Admin);
                await _userRepository.AddAsync(user, cancellationToken);
            }

            return new AuthResponse(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email.Value,
                user.Role.ToString(),
                session.AccessToken
            );
        }
        catch (Exception)
        {
            // Supabase auth failed
            return null;
        }
    }
}
