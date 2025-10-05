using MediatR;
using SportPlanner.Application.DTOs;

namespace SportPlanner.Application.UseCases;

public record CreateGenderCommand(
    string Name,
    string Code,
    string? Description,
    bool IsActive
) : IRequest<GenderResponse>;
