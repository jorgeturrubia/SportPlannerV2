using MediatR;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases;

public class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand, bool>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISubscriptionRepository _subscriptionRepository;

    public UpdateTeamCommandHandler(
        ITeamRepository teamRepository,
        ICurrentUserService currentUserService,
        ISubscriptionRepository subscriptionRepository)
    {
        _teamRepository = teamRepository;
        _currentUserService = currentUserService;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<bool> Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar que subscription existe y está activa
        var subscription = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId, cancellationToken);
        if (subscription == null || !subscription.IsActive)
            throw new InvalidOperationException("Subscription not found or inactive");

        // 2. Validar autorización - usuario debe ser owner de la subscription
        var currentUserId = _currentUserService.GetUserId();
        if (subscription.OwnerId != currentUserId)
            throw new UnauthorizedAccessException("User not authorized to update teams in this subscription");

        // 3. Obtener el equipo
        var team = await _teamRepository.GetByIdAsync(request.TeamId, cancellationToken);
        if (team == null)
            return false;

        // 4. Validar que el equipo pertenece a la subscription
        if (team.SubscriptionId != request.SubscriptionId)
            throw new UnauthorizedAccessException("Team does not belong to this subscription");

        // 5. Validar nombre único dentro de la subscription (si cambió)
        if (team.Name != request.Name)
        {
            if (await _teamRepository.ExistsWithNameInSubscriptionAsync(request.SubscriptionId, request.Name, cancellationToken, request.TeamId))
                throw new InvalidOperationException($"Team with name '{request.Name}' already exists in this subscription");
        }

        // 6. Actualizar información básica
        team.UpdateBasicInfo(request.Name, request.Color, request.Description);

        await _teamRepository.UpdateAsync(team, cancellationToken);
        return true;
    }
}
