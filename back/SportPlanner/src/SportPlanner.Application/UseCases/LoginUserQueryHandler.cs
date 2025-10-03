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
                // First time login - create local user record from Supabase data
                var email = Email.Create(request.Email);
                var supabaseUser = session.User;

                // Extract name from metadata if available, otherwise use email prefix
                var firstName = supabaseUser.UserMetadata?.TryGetValue("first_name", out var fName) == true && fName is not null 
                    ? fName.ToString() 
                    : "User";
                var lastName = supabaseUser.UserMetadata?.TryGetValue("last_name", out var lName) == true && lName is not null 
                    ? lName.ToString() 
                    : supabaseUser.Email?.Split('@')[0] ?? "Unknown";

                // Create user with Supabase ID for tracking
                var supabaseUserId = supabaseUser.Id ?? throw new InvalidOperationException("Supabase user ID is null");
                
                user = new SportPlanner.Domain.Entities.User(
                    firstName,
                    lastName,
                    email,
                    SportPlanner.Domain.Entities.UserRole.Admin,
                    supabaseUserId  // Sync Supabase User ID
                );
                await _userRepository.AddAsync(user, cancellationToken);
            }
            else if (string.IsNullOrEmpty(user.SupabaseUserId))
            {
                // User exists locally but doesn't have Supabase ID - sync it
                var supabaseUserId = session.User.Id ?? throw new InvalidOperationException("Supabase user ID is null");
                user.SyncSupabaseUserId(supabaseUserId);
                await _userRepository.UpdateAsync(user, cancellationToken);
            }

            return new AuthResponse(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email.Value,
                user.Role.ToString(),
                session.AccessToken ?? throw new InvalidOperationException("Access token is null after successful login"),
                session.RefreshToken
            );
        }
        catch (Exception ex)
        {
            // Propagate Supabase auth error with details
            throw new UnauthorizedAccessException($"Authentication failed: {ex.Message}", ex);
        }
    }
}
