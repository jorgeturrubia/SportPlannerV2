using MediatR;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

public class GetObjectiveSubcategoriesQueryHandler : IRequestHandler<GetObjectiveSubcategoriesQuery, List<ObjectiveSubcategoryDto>>
{
    private readonly IObjectiveSubcategoryRepository _repository;

    public GetObjectiveSubcategoriesQueryHandler(IObjectiveSubcategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ObjectiveSubcategoryDto>> Handle(GetObjectiveSubcategoriesQuery request, CancellationToken cancellationToken)
    {
        var subcategories = request.CategoryId.HasValue
            ? await _repository.GetByCategoryIdAsync(request.CategoryId.Value, cancellationToken)
            : await _repository.GetAllAsync(cancellationToken);

        return subcategories.Select(s => new ObjectiveSubcategoryDto
        {
            Id = s.Id,
            ObjectiveCategoryId = s.ObjectiveCategoryId,
            Name = s.Name
        }).ToList();
    }
}
