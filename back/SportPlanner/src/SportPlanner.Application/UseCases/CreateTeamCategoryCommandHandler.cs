using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities;

namespace SportPlanner.Application.UseCases;

public class CreateTeamCategoryCommandHandler : IRequestHandler<CreateTeamCategoryCommand, TeamCategoryResponse>
{
    private readonly ITeamCategoryRepository _teamCategoryRepository;

    public CreateTeamCategoryCommandHandler(ITeamCategoryRepository teamCategoryRepository)
    {
        _teamCategoryRepository = teamCategoryRepository;
    }

    public async Task<TeamCategoryResponse> Handle(CreateTeamCategoryCommand request, CancellationToken cancellationToken)
    {
        var teamCategory = new TeamCategory(
            request.Name,
            request.Code,
            request.Sport,
            request.Description,
            request.SortOrder
        );

        if (!request.IsActive)
            teamCategory.Deactivate();

        await _teamCategoryRepository.AddAsync(teamCategory, cancellationToken);

        return new TeamCategoryResponse(
            teamCategory.Id,
            teamCategory.Name,
            teamCategory.Code,
            teamCategory.Description,
            teamCategory.SortOrder,
            teamCategory.Sport,
            teamCategory.IsActive
        );
    }
}
