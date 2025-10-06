using MediatR;
using SportPlanner.Application.DTOs.Planning;

namespace SportPlanner.Application.UseCases.Planning;

public record GetObjectiveSubcategoriesQuery(Guid? CategoryId = null) : IRequest<List<ObjectiveSubcategoryDto>>;
