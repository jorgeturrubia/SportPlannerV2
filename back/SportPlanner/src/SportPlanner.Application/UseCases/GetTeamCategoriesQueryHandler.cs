using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases;

public class GetTeamCategoriesQueryHandler : IRequestHandler<GetTeamCategoriesQuery, List<TeamCategoryResponse>>
{
    private readonly ITeamCategoryRepository _teamCategoryRepository;

    public GetTeamCategoriesQueryHandler(ITeamCategoryRepository teamCategoryRepository)
    {
        _teamCategoryRepository = teamCategoryRepository;
    }

    public async Task<List<TeamCategoryResponse>> Handle(GetTeamCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = request.Sport.HasValue
            ? await _teamCategoryRepository.GetActiveBySportAsync(request.Sport.Value, cancellationToken)
            : await _teamCategoryRepository.GetAllActiveAsync(cancellationToken);

        return categories.Select(c => new TeamCategoryResponse(
            c.Id,
            c.Name,
            c.Code,
            c.Description,
            c.SortOrder,
            c.Sport,
            c.IsActive
        )).ToList();
    }
}