using MediatR;
using SportPlanner.Application.DTOs;

namespace SportPlanner.Application.UseCases;

public record LoginUserQuery(
    string Email,
    string Password
) : IRequest<AuthResponse>;
