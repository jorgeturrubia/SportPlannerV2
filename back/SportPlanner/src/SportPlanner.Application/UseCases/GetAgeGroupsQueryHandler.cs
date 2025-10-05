using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases;

public class GetAgeGroupsQueryHandler : IRequestHandler<GetAgeGroupsQuery, List<AgeGroupResponse>>
{
    private readonly IAgeGroupRepository _ageGroupRepository;

    public GetAgeGroupsQueryHandler(IAgeGroupRepository ageGroupRepository)
    {
        _ageGroupRepository = ageGroupRepository;
    }

    public async Task<List<AgeGroupResponse>> Handle(GetAgeGroupsQuery request, CancellationToken cancellationToken)
    {
        var ageGroups = request.Sport.HasValue
            ? await _ageGroupRepository.GetActiveBySportAsync(request.Sport.Value, cancellationToken)
            : await _ageGroupRepository.GetAllActiveAsync(cancellationToken);

        return ageGroups.Select(a => new AgeGroupResponse(
            a.Id,
            a.Name,
            a.Code,
            a.MinAge,
            a.MaxAge,
            a.Sport,
            a.SortOrder,
            a.IsActive
        )).ToList();
    }
}
