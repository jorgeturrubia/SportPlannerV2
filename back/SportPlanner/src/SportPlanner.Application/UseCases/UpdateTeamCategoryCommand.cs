using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.UseCases;

public record UpdateTeamCategoryCommand(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    Sport Sport,
    int SortOrder,
    bool IsActive
) : IRequest<TeamCategoryResponse>;
