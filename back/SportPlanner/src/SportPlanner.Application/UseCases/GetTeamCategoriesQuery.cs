using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.UseCases;

public record GetTeamCategoriesQuery(Sport? Sport = null) : IRequest<List<TeamCategoryResponse>>;