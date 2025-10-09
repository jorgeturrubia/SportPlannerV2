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
        Supabase.Gotrue.Session? session = null;

        try
        {
            // Step 1: Create user in Supabase Auth first (this validates email, password strength, etc.)
            session = await _authService.SignUpAsync(request.Email, request.Password);

            if (session?.User is null)
            {
                // Supabase sign-up failed or returned no user (e.g., existing user or invalid data)
                // Return null so callers/tests can handle this scenario gracefully.
                return null;
            }

            // Step 2: Extract Supabase user info
            var supabaseUserId = session.User.Id ?? throw new InvalidOperationException("Supabase user ID is null");
            var email = SportPlanner.Domain.ValueObjects.Email.Create(request.Email);

            // Step 3: Check if user already exists in local database (by email)
            var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (existingUser is not null)
            {
                // User already exists locally, just sync Supabase ID and return
                if (string.IsNullOrEmpty(existingUser.SupabaseUserId))
                {
                    existingUser.SyncSupabaseUserId(supabaseUserId);
                    await _userRepository.UpdateAsync(existingUser, cancellationToken);
                }

                return new AuthResponse(
                    existingUser.Id,
                    existingUser.FirstName,
                    existingUser.LastName,
                    existingUser.Email.Value,
                    existingUser.Role.ToString(),
                    session.AccessToken ?? string.Empty,
                    session.RefreshToken
                );
            }

            // Step 4: Create new user in local database with Supabase sync
            var user = new User(
                request.FirstName,
                request.LastName,
                email,
                UserRole.Admin, // Default role for new users
                supabaseUserId  // Link to Supabase Auth user
            );

            // Step 5: Persist user to local database
            await _userRepository.AddAsync(user, cancellationToken);

            // Step 6: Return successful auth response
            // Note: AccessToken might be null if email confirmation is required in Supabase
            return new AuthResponse(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email.Value,
                user.Role.ToString(),
                session.AccessToken ?? string.Empty, // Empty string if confirmation required
                session.RefreshToken
            );
        }
        catch (Exception ex)
        {
            // If local database save fails but Supabase user was created, log for manual cleanup
            if (session?.User is not null)
            {
                // TODO: Implement cleanup job or admin notification
                // For now, the user exists in Supabase but not in local DB
                // They can login later and we'll sync in LoginUserQueryHandler
                throw new InvalidOperationException(
                    $"Registration failed: User created in Supabase but local database sync failed. {ex.Message}",
                    ex
                );
            }

            // Propagate Supabase registration error with details
            throw new InvalidOperationException($"Registration failed: {ex.Message}", ex);
        }
    }
}
