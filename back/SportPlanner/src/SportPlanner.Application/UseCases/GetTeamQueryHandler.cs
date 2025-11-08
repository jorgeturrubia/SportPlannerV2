using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases;

public class GetTeamQueryHandler : IRequestHandler<GetTeamQuery, TeamResponse?>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ISubscriptionUserRepository _subscriptionUserRepository;
    private readonly IUserRepository _userRepository;

    public GetTeamQueryHandler(
        ITeamRepository teamRepository,
        ISubscriptionUserRepository subscriptionUserRepository,
        IUserRepository userRepository)
    {
        _teamRepository = teamRepository;
        _subscriptionUserRepository = subscriptionUserRepository;
        _userRepository = userRepository;
    }

    public async Task<TeamResponse?> Handle(GetTeamQuery request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdWithDetailsAsync(request.TeamId, cancellationToken);

        // Verificar que el team pertenece a la subscription solicitada
        if (team == null || team.SubscriptionId != request.SubscriptionId)
        {
            return null;
        }

        // Enrich coach data if CoachSubscriptionUserId exists
        string? coachFirstName = null;
        string? coachLastName = null;
        string? coachEmail = null;

        if (team.CoachSubscriptionUserId.HasValue)
        {
            var coachSubscriptionUser = await _subscriptionUserRepository.GetByIdAsync(team.CoachSubscriptionUserId.Value, cancellationToken);
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

        return new TeamResponse(
            team.Id,
            team.SubscriptionId,
            team.Name,
            team.Color,
            team.SportId,
            team.Description,
            team.CoachSubscriptionUserId,
            coachFirstName,
            coachLastName,
            coachEmail,
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
                team.Category.SportId,
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
                team.AgeGroup.SportId,
                team.AgeGroup.SortOrder,
                team.AgeGroup.IsActive
            ),
            team.CreatedAt
        );
    }
}
