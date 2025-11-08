using MediatR;
using SportPlanner.Application.DTOs;

namespace SportPlanner.Application.UseCases;

public record GetTeamCategoriesQuery(Guid? SportId = null) : IRequest<List<TeamCategoryResponse>>;