using MediatR;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Domain.Enum;
using SportPlanner.Shared.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SportPlanner.Application.UseCases.Planning;

public class PublishToMarketplaceCommandHandler : IRequestHandler<PublishToMarketplaceCommand, Guid>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IMarketplaceItemRepository _marketplaceItemRepository;
    private readonly ITrainingPlanRepository _trainingPlanRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IObjectiveRepository _objectiveRepository;

    public PublishToMarketplaceCommandHandler(
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

    public async Task<Guid> Handle(PublishToMarketplaceCommand request, CancellationToken cancellationToken)
    {
        var subscriptionId = _currentUserService.GetSubscriptionId();
        if (subscriptionId == Guid.Empty)
        {
            throw new UnauthorizedException("User is not associated with a subscription.");
        }

        var userId = _currentUserService.GetUserId().ToString();
        MarketplaceItem marketplaceItem;

        switch (request.Type)
        {
            case MarketplaceItemType.TrainingPlan:
                var plan = await _trainingPlanRepository.GetByIdAsync(request.SourceEntityId, cancellationToken)
                    ?? throw new NotFoundException($"TrainingPlan with ID '{request.SourceEntityId}' not found.");

                if (plan.SubscriptionId != subscriptionId)
                {
                    throw new ForbiddenException("You can only publish resources from your own subscription.");
                }

                marketplaceItem = MarketplaceItem.CreateUserItem(
                    request.Type,
                    plan.Schedule.Sport, // Assuming Sport is on the schedule
                    request.SourceEntityId,
                    subscriptionId,
                    plan.Name,
                    "A comprehensive training plan.", // Placeholder description
                    userId);

                break;

            case MarketplaceItemType.Exercise:
                var exercise = await _exerciseRepository.GetByIdAsync(request.SourceEntityId, cancellationToken)
                    ?? throw new NotFoundException($"Exercise with ID '{request.SourceEntityId}' not found.");

                if (exercise.SubscriptionId != subscriptionId)
                {
                    throw new ForbiddenException("You can only publish resources from your own subscription.");
                }

                marketplaceItem = MarketplaceItem.CreateUserItem(
                    request.Type,
                    exercise.Sport,
                    request.SourceEntityId,
                    subscriptionId,
                    exercise.Name,
                    exercise.Description,
                    userId);

                break;

            case MarketplaceItemType.Objective:
                var objective = await _objectiveRepository.GetByIdAsync(request.SourceEntityId, cancellationToken)
                    ?? throw new NotFoundException($"Objective with ID '{request.SourceEntityId}' not found.");

                if (objective.SubscriptionId != subscriptionId)
                {
                    throw new ForbiddenException("You can only publish resources from your own subscription.");
                }

                marketplaceItem = MarketplaceItem.CreateUserItem(
                    request.Type,
                    objective.Sport,
                    request.SourceEntityId,
                    subscriptionId,
                    objective.Name,
                    objective.Description,
                    userId);

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(request.Type), "The specified item type cannot be published.");
        }

        await _marketplaceItemRepository.AddAsync(marketplaceItem, cancellationToken);

        return marketplaceItem.Id;
    }
}