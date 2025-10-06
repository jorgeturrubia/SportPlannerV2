using MediatR;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.UseCases.Planning;

public record GetObjectiveCategoriesQuery(Sport? Sport = null) : IRequest<List<ObjectiveCategoryDto>>;
