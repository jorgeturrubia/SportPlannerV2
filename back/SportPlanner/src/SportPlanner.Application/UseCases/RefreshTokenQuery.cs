using MediatR;
using SportPlanner.Application.DTOs;

namespace SportPlanner.Application.UseCases;

public record RefreshTokenQuery(string RefreshToken) : IRequest<AuthResponse?>;
