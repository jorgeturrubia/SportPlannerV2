using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases;

public class GetTeamQueryHandler : IRequestHandler<GetTeamQuery, TeamResponse?>
{
    private readonly ITeamRepository _teamRepository;

    public GetTeamQueryHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<TeamResponse?> Handle(GetTeamQuery request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdWithDetailsAsync(request.TeamId, cancellationToken);

        // Verificar que el team pertenece a la subscription solicitada
        if (team == null || team.SubscriptionId != request.SubscriptionId)
        {
            return null;
        }

        return new TeamResponse(
            team.Id,
            team.SubscriptionId,
            team.Name,
            team.Color,
            team.Sport,
            team.Description,
            team.HomeVenue,
            team.CoachName,
            team.ContactEmail,
            team.ContactPhone,
            team.Season,
            team.MaxPlayers,
            team.CurrentPlayersCount,
            team.LastMatchDate,
            team.AllowMixedGender,
            team.IsActive,
            new TeamCategoryResponse(
                team.Category.Id,
                team.Category.Name,
                team.Category.Code,
                team.Category.Description,
                team.Category.SortOrder,
                team.Category.Sport,
                team.Category.IsActive
            ),
            new GenderResponse(
                team.Gender.Id,
                team.Gender.Name,
                team.Gender.Code,
                team.Gender.Description,
                team.Gender.IsActive
            ),
            new AgeGroupResponse(
                team.AgeGroup.Id,
                team.AgeGroup.Name,
                team.AgeGroup.Code,
                team.AgeGroup.MinAge,
                team.AgeGroup.MaxAge,
                team.AgeGroup.Sport,
                team.AgeGroup.SortOrder,
                team.AgeGroup.IsActive
            ),
            team.CreatedAt
        );
    }
}
