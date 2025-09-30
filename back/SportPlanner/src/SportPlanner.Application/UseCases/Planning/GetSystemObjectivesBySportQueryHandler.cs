using MediatR;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

public class GetSystemObjectivesBySportQueryHandler : IRequestHandler<GetSystemObjectivesBySportQuery, List<ObjectiveDto>>
{
    private readonly IObjectiveRepository _objectiveRepository;

    public GetSystemObjectivesBySportQueryHandler(IObjectiveRepository objectiveRepository)
    {
        _objectiveRepository = objectiveRepository;
    }

    public async Task<List<ObjectiveDto>> Handle(GetSystemObjectivesBySportQuery request, CancellationToken cancellationToken)
    {
        // Get system objectives for the specified sport
        // TODO: Add caching layer here (IDistributedCache) for performance
        var objectives = await _objectiveRepository.GetSystemObjectivesBySportAsync(request.Sport, cancellationToken);

        // Map to DTOs
        return objectives.Select(o => new ObjectiveDto
        {
            Id = o.Id,
            SubscriptionId = o.SubscriptionId,
            Ownership = o.Ownership,
            Sport = o.Sport,
            Name = o.Name,
            Description = o.Description,
            ObjectiveCategoryId = o.ObjectiveCategoryId,
            ObjectiveSubcategoryId = o.ObjectiveSubcategoryId,
            IsActive = o.IsActive,
            SourceMarketplaceItemId = o.SourceMarketplaceItemId,
            Techniques = o.Techniques.Select(t => new ObjectiveTechniqueDto
            {
                Description = t.Description,
                Order = t.Order
            }).ToList(),
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt
        }).ToList();
    }
}