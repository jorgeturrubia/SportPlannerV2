using MediatR;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Domain.Enum;
using SportPlanner.Shared.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SportPlanner.Application.UseCases.Planning;

public class DownloadFromMarketplaceCommandHandler : IRequestHandler<DownloadFromMarketplaceCommand, Guid>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IMarketplaceItemRepository _marketplaceItemRepository;
    private readonly ITrainingPlanRepository _trainingPlanRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IObjectiveRepository _objectiveRepository;

    public DownloadFromMarketplaceCommandHandler(
        ICurrentUserService currentUserService,
        IMarketplaceItemRepository marketplaceItemRepository,
        ITrainingPlanRepository trainingPlanRepository,
        IExerciseRepository exerciseRepository,
        IObjectiveRepository objectiveRepository)
    {
        _currentUserService = currentUserService;
        _marketplaceItemRepository = marketplaceItemRepository;
        _trainingPlanRepository = trainingPlanRepository;
        _exerciseRepository = exerciseRepository;
        _objectiveRepository = objectiveRepository;
    }

    public async Task<Guid> Handle(DownloadFromMarketplaceCommand request, CancellationToken cancellationToken)
    {
        var subscriptionId = _currentUserService.GetSubscriptionId();
        if (subscriptionId == Guid.Empty)
        {
            throw new UnauthorizedException("User is not associated with a subscription.");
        }

        var userId = _currentUserService.GetUserId().ToString();

        var marketplaceItem = await _marketplaceItemRepository.GetByIdAsync(request.MarketplaceItemId, cancellationToken)
            ?? throw new NotFoundException($"MarketplaceItem with ID '{request.MarketplaceItemId}' not found.");

        if (marketplaceItem.SourceEntityId is null)
        {
            throw new InvalidOperationException("Marketplace item does not have a valid source entity to download.");
        }

        Guid newEntityId;

        switch (marketplaceItem.Type)
        {
            case MarketplaceItemType.TrainingPlan:
                var sourcePlan = await _trainingPlanRepository.GetByIdAsync(marketplaceItem.SourceEntityId.Value, cancellationToken)
                    ?? throw new NotFoundException($"Source TrainingPlan with ID '{marketplaceItem.SourceEntityId}' not found.");

                var newPlan = sourcePlan.Clone(subscriptionId, userId);
                await _trainingPlanRepository.AddAsync(newPlan, cancellationToken);
                newEntityId = newPlan.Id;
                break;

            case MarketplaceItemType.Exercise:
                var sourceExercise = await _exerciseRepository.GetByIdAsync(marketplaceItem.SourceEntityId.Value, cancellationToken)
                    ?? throw new NotFoundException($"Source Exercise with ID '{marketplaceItem.SourceEntityId}' not found.");

                var newExercise = sourceExercise.Clone(subscriptionId, userId);
                await _exerciseRepository.AddAsync(newExercise, cancellationToken);
                newEntityId = newExercise.Id;
                break;

            case MarketplaceItemType.Objective:
                var sourceObjective = await _objectiveRepository.GetByIdAsync(marketplaceItem.SourceEntityId.Value, cancellationToken)
                    ?? throw new NotFoundException($"Source Objective with ID '{marketplaceItem.SourceEntityId}' not found.");

                var newObjective = sourceObjective.Clone(subscriptionId);
                await _objectiveRepository.AddAsync(newObjective, cancellationToken);
                newEntityId = newObjective.Id;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(marketplaceItem.Type), "The specified item type cannot be downloaded.");
        }

        marketplaceItem.IncrementDownloads();
        await _marketplaceItemRepository.UpdateAsync(marketplaceItem, cancellationToken);

        return newEntityId;
    }
}