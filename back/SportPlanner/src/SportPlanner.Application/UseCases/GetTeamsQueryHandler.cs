using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases;

public class GetTeamsQueryHandler : IRequestHandler<GetTeamsQuery, List<TeamResponse>>
{
    private readonly ITeamRepository _teamRepository;

    public GetTeamsQueryHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<List<TeamResponse>> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
    {
        var teams = request.ActiveOnly
            ? await _teamRepository.GetActiveBySubscriptionIdAsync(request.SubscriptionId, cancellationToken)
            : await _teamRepository.GetBySubscriptionIdAsync(request.SubscriptionId, cancellationToken);

        return teams.Select(t => new TeamResponse(
            t.Id,
            t.SubscriptionId,
            t.Name,
            t.Color,
            t.Sport,
            t.Description,
            t.HomeVenue,
            t.CoachName,
            t.ContactEmail,
            t.ContactPhone,
            t.Season,
            t.MaxPlayers,
            t.CurrentPlayersCount,
            t.LastMatchDate,
            t.AllowMixedGender,
            t.IsActive,
            new TeamCategoryResponse(
                t.Category.Id,
                t.Category.Name,
                t.Category.Code,
                t.Category.Description,
                t.Category.SortOrder,
                t.Category.Sport,
                t.Category.IsActive
            ),
            new GenderResponse(
                t.Gender.Id,
                t.Gender.Name,
                t.Gender.Code,
                t.Gender.Description,
                t.Gender.IsActive
            ),
            new AgeGroupResponse(
                t.AgeGroup.Id,
                t.AgeGroup.Name,
                t.AgeGroup.Code,
                t.AgeGroup.MinAge,
                t.AgeGroup.MaxAge,
                t.AgeGroup.Sport,
                t.AgeGroup.SortOrder,
                t.AgeGroup.IsActive
            ),
            t.CreatedAt
        )).ToList();
    }
}
