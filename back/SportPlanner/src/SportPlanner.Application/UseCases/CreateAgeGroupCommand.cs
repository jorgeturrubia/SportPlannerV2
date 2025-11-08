using MediatR;
using SportPlanner.Application.DTOs;

namespace SportPlanner.Application.UseCases;

public record CreateAgeGroupCommand(
    string Name,
    string Code,
    int MinAge,
    int MaxAge,
    Guid SportId,
    int SortOrder,
    bool IsActive
) : IRequest<AgeGroupResponse>;
