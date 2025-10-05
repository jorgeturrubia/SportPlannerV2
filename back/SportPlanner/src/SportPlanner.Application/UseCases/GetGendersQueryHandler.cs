using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases;

public class GetGendersQueryHandler : IRequestHandler<GetGendersQuery, List<GenderResponse>>
{
    private readonly IGenderRepository _genderRepository;

    public GetGendersQueryHandler(IGenderRepository genderRepository)
    {
        _genderRepository = genderRepository;
    }

    public async Task<List<GenderResponse>> Handle(GetGendersQuery request, CancellationToken cancellationToken)
    {
        var genders = await _genderRepository.GetAllActiveAsync(cancellationToken);

        return genders.Select(g => new GenderResponse(
            g.Id,
            g.Name,
            g.Code,
            g.Description,
            g.IsActive
        )).ToList();
    }
}
