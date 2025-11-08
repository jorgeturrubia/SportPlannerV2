using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases;

public class UpdateTeamCategoryCommandHandler : IRequestHandler<UpdateTeamCategoryCommand, TeamCategoryResponse>
{
    private readonly ITeamCategoryRepository _teamCategoryRepository;

    public UpdateTeamCategoryCommandHandler(ITeamCategoryRepository teamCategoryRepository)
    {
        _teamCategoryRepository = teamCategoryRepository;
    }

    public async Task<TeamCategoryResponse> Handle(UpdateTeamCategoryCommand request, CancellationToken cancellationToken)
    {
        var teamCategory = await _teamCategoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (teamCategory == null)
        {
            throw new KeyNotFoundException($"Team category with ID {request.Id} not found");
        }

        teamCategory.UpdateDetails(request.Name, request.Description, request.SortOrder);

        if (request.IsActive)
            teamCategory.Activate();
        else
            teamCategory.Deactivate();

        await _teamCategoryRepository.UpdateAsync(teamCategory, cancellationToken);

        return new TeamCategoryResponse(
            teamCategory.Id,
            teamCategory.Name,
            teamCategory.Code,
            teamCategory.Description,
            teamCategory.SortOrder,
            teamCategory.SportId,
            teamCategory.IsActive
        );
    }
}
