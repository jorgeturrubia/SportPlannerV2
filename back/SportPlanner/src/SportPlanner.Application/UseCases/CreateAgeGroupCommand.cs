using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.UseCases;

public record CreateAgeGroupCommand(
    string Name,
    string Code,
    int MinAge,
    int MaxAge,
    Sport Sport,
    int SortOrder,
    bool IsActive
) : IRequest<AgeGroupResponse>;
