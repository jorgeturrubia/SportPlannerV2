using MediatR;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases;

public class DeleteTeamCommandHandler : IRequestHandler<DeleteTeamCommand, bool>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISubscriptionRepository _subscriptionRepository;

    public DeleteTeamCommandHandler(
        ITeamRepository teamRepository,
        ICurrentUserService currentUserService,
        ISubscriptionRepository subscriptionRepository)
    {
        _teamRepository = teamRepository;
        _currentUserService = currentUserService;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<bool> Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar que subscription existe y está activa
        var subscription = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId, cancellationToken);
        if (subscription == null || !subscription.IsActive)
            throw new InvalidOperationException("Subscription not found or inactive");

        // 2. Validar autorización - usuario debe ser owner de la subscription
        var currentUserId = _currentUserService.GetUserId();
        if (subscription.OwnerId != currentUserId)
            throw new UnauthorizedAccessException("User not authorized to delete teams in this subscription");

        // 3. Obtener el equipo
        var team = await _teamRepository.GetByIdAsync(request.TeamId, cancellationToken);
        if (team == null)
            return false;

        // 4. Validar que el equipo pertenece a la subscription
        if (team.SubscriptionId != request.SubscriptionId)
            throw new UnauthorizedAccessException("Team does not belong to this subscription");

        // 5. Eliminar el equipo
        await _teamRepository.DeleteAsync(team, cancellationToken);
        return true;
    }
}
