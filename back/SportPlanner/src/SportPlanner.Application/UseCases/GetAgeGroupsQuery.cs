using MediatR;
using SportPlanner.Application.DTOs;

namespace SportPlanner.Application.UseCases;

public record GetAgeGroupsQuery(Guid? SportId = null) : IRequest<List<AgeGroupResponse>>;
