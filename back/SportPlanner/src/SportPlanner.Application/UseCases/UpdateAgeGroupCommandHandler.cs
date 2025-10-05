using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases;

public class UpdateAgeGroupCommandHandler : IRequestHandler<UpdateAgeGroupCommand, AgeGroupResponse>
{
    private readonly IAgeGroupRepository _ageGroupRepository;

    public UpdateAgeGroupCommandHandler(IAgeGroupRepository ageGroupRepository)
    {
        _ageGroupRepository = ageGroupRepository;
    }

    public async Task<AgeGroupResponse> Handle(UpdateAgeGroupCommand request, CancellationToken cancellationToken)
    {
        var ageGroup = await _ageGroupRepository.GetByIdAsync(request.Id, cancellationToken);
        if (ageGroup == null)
        {
            throw new KeyNotFoundException($"Age group with ID {request.Id} not found");
        }

        ageGroup.UpdateDetails(request.Name, request.MinAge, request.MaxAge, request.SortOrder);

        if (request.IsActive)
            ageGroup.Activate();
        else
            ageGroup.Deactivate();

        await _ageGroupRepository.UpdateAsync(ageGroup, cancellationToken);

        return new AgeGroupResponse(
            ageGroup.Id,
            ageGroup.Name,
            ageGroup.Code,
            ageGroup.MinAge,
            ageGroup.MaxAge,
            ageGroup.Sport,
            ageGroup.SortOrder,
            ageGroup.IsActive
        );
    }
}
