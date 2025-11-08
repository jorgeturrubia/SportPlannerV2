using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities;

namespace SportPlanner.Application.UseCases;

public class CreateAgeGroupCommandHandler : IRequestHandler<CreateAgeGroupCommand, AgeGroupResponse>
{
    private readonly IAgeGroupRepository _ageGroupRepository;

    public CreateAgeGroupCommandHandler(IAgeGroupRepository ageGroupRepository)
    {
        _ageGroupRepository = ageGroupRepository;
    }

    public async Task<AgeGroupResponse> Handle(CreateAgeGroupCommand request, CancellationToken cancellationToken)
    {
        var ageGroup = new AgeGroup(
            request.Name,
            request.Code,
            request.MinAge,
            request.MaxAge,
            request.SportId,
            request.SortOrder
        );

        if (!request.IsActive)
            ageGroup.Deactivate();

        await _ageGroupRepository.AddAsync(ageGroup, cancellationToken);

        return new AgeGroupResponse(
            ageGroup.Id,
            ageGroup.Name,
            ageGroup.Code,
            ageGroup.MinAge,
            ageGroup.MaxAge,
            ageGroup.SportId,
            ageGroup.SortOrder,
            ageGroup.IsActive
        );
    }
}
