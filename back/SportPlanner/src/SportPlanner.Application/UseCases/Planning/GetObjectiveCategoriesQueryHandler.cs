using MediatR;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

public class GetObjectiveCategoriesQueryHandler : IRequestHandler<GetObjectiveCategoriesQuery, List<ObjectiveCategoryDto>>
{
    private readonly IObjectiveCategoryRepository _repository;

    public GetObjectiveCategoriesQueryHandler(IObjectiveCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ObjectiveCategoryDto>> Handle(GetObjectiveCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = request.Sport.HasValue
            ? await _repository.GetBySportAsync(request.Sport.Value, cancellationToken)
            : await _repository.GetAllAsync(cancellationToken);

        return categories.Select(c => new ObjectiveCategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Sport = c.Sport
        }).ToList();
    }
}
