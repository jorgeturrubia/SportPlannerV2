using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.UseCases;

public record GetAgeGroupsQuery(Sport? Sport = null) : IRequest<List<AgeGroupResponse>>;
