using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases;

public class GetTeamsQueryHandler : IRequestHandler<GetTeamsQuery, List<TeamResponse>>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ISubscriptionUserRepository _subscriptionUserRepository;
    private readonly IUserRepository _userRepository;

    public GetTeamsQueryHandler(
        ITeamRepository teamRepository,
        ISubscriptionUserRepository subscriptionUserRepository,
        IUserRepository userRepository)
    {
        _teamRepository = teamRepository;
        _subscriptionUserRepository = subscriptionUserRepository;
        _userRepository = userRepository;
    }

    public async Task<List<TeamResponse>> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
    {
        var teams = request.ActiveOnly
            ? await _teamRepository.GetActiveBySubscriptionIdAsync(request.SubscriptionId, cancellationToken)
            : await _teamRepository.GetBySubscriptionIdAsync(request.SubscriptionId, cancellationToken);

        var teamResponses = new List<TeamResponse>();

        foreach (var t in teams)
        {
            // Enrich coach data if CoachSubscriptionUserId exists
            string? coachFirstName = null;
            string? coachLastName = null;
            string? coachEmail = null;

            if (t.CoachSubscriptionUserId.HasValue)
            {
                var coachSubscriptionUser = await _subscriptionUserRepository.GetByIdAsync(t.CoachSubscriptionUserId.Value, cancellationToken);
                if (coachSubscriptionUser != null)
                {
                    var coachUser = await _userRepository.GetByIdAsync(coachSubscriptionUser.UserId, cancellationToken);
                    if (coachUser != null)
                    {
                        coachFirstName = coachUser.FirstName;
                        coachLastName = coachUser.LastName;
                        coachEmail = coachUser.Email.Value;
                    }
                }
            }

            teamResponses.Add(new TeamResponse(
                t.Id,
                t.SubscriptionId,
                t.Name,
                t.Color,
                t.Sport,
                t.Description,
                t.CoachSubscriptionUserId,
                coachFirstName,
                coachLastName,
                coachEmail,
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
            ));
        }

        return teamResponses;
    }
}
