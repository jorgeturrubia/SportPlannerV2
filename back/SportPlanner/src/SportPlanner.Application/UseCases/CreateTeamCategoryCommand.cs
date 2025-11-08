using MediatR;
using SportPlanner.Application.DTOs;

namespace SportPlanner.Application.UseCases;

public record CreateTeamCategoryCommand(
    string Name,
    string Code,
    string? Description,
    Guid SportId,
    int SortOrder,
    bool IsActive
) : IRequest<TeamCategoryResponse>;
