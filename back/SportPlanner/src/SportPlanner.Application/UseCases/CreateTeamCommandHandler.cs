using MediatR;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities;

namespace SportPlanner.Application.UseCases;

public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, Guid>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ITeamCategoryRepository _teamCategoryRepository;
    private readonly IGenderRepository _genderRepository;
    private readonly IAgeGroupRepository _ageGroupRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateTeamCommandHandler(
        ITeamRepository teamRepository,
        ISubscriptionRepository subscriptionRepository,
        ITeamCategoryRepository teamCategoryRepository,
        IGenderRepository genderRepository,
        IAgeGroupRepository ageGroupRepository,
        ICurrentUserService currentUserService)
    {
        _teamRepository = teamRepository;
        _subscriptionRepository = subscriptionRepository;
        _teamCategoryRepository = teamCategoryRepository;
        _genderRepository = genderRepository;
        _ageGroupRepository = ageGroupRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar que subscription existe y está activa
        var subscription = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId, cancellationToken);
        if (subscription == null || !subscription.IsActive)
            throw new InvalidOperationException("Subscription not found or inactive");

        // 2. Validar autorización - usuario debe ser owner de la subscription
        var currentUserId = _currentUserService.GetUserId();
        if (subscription.OwnerId != currentUserId)
            throw new UnauthorizedAccessException("User not authorized to create teams in this subscription");

        // 3. Validar límite de equipos
        var currentTeamCount = await _teamRepository.CountActiveTeamsBySubscriptionAsync(request.SubscriptionId, cancellationToken);
        if (currentTeamCount >= subscription.MaxTeams)
            throw new InvalidOperationException($"Maximum team limit ({subscription.MaxTeams}) reached for this subscription");

        // 4. Validar nombre único dentro de la subscription
        if (await _teamRepository.ExistsWithNameInSubscriptionAsync(request.SubscriptionId, request.Name, cancellationToken))
            throw new InvalidOperationException($"Team with name '{request.Name}' already exists in this subscription");

        // 5. Validar entidades maestras existen y están activas
        var category = await _teamCategoryRepository.GetByIdAsync(request.TeamCategoryId, cancellationToken);
        if (category == null || !category.IsActive)
            throw new InvalidOperationException("Team category not found or inactive");

        var gender = await _genderRepository.GetByIdAsync(request.GenderId, cancellationToken);
        if (gender == null || !gender.IsActive)
            throw new InvalidOperationException("Gender not found or inactive");

        var ageGroup = await _ageGroupRepository.GetByIdAsync(request.AgeGroupId, cancellationToken);
        if (ageGroup == null || !ageGroup.IsActive)
            throw new InvalidOperationException("Age group not found or inactive");

        // 6. Validar coherencia entre maestros y subscription
        if (category.Sport != subscription.Sport)
            throw new InvalidOperationException("Team category sport does not match subscription sport");

        if (ageGroup.Sport != subscription.Sport)
            throw new InvalidOperationException("Age group sport does not match subscription sport");

        // 7. Validar género mixto
        if (request.AllowMixedGender && gender.Code != "X")
            throw new InvalidOperationException("Mixed gender is only allowed when gender is set to 'Mixto'");

        // 8. Crear equipo con datos validados
        var team = new Team(
            request.SubscriptionId,
            request.Name,
            request.Color,
            request.TeamCategoryId,
            request.GenderId,
            request.AgeGroupId,
            subscription.Sport,
            request.Description,
            request.HomeVenue,
            request.CoachName,
            request.ContactEmail,
            request.ContactPhone,
            request.Season,
            request.AllowMixedGender);

        await _teamRepository.AddAsync(team, cancellationToken);

        return team.Id;
    }
}