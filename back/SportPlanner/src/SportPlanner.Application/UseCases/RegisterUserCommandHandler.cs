using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.ValueObjects;

namespace SportPlanner.Application.UseCases;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public RegisterUserCommandHandler(IUserRepository userRepository, IAuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Check if email already exists
        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        // Create user
        var email = Email.Create(request.Email);
        var passwordHash = _authService.HashPassword(request.Password);
        var user = new User(request.FirstName, request.LastName, email, passwordHash, UserRole.Admin);

        // Save user
        await _userRepository.AddAsync(user, cancellationToken);

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
