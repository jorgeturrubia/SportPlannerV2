using MediatR;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases;

public class DeleteAgeGroupCommandHandler : IRequestHandler<DeleteAgeGroupCommand, bool>
{
    private readonly IAgeGroupRepository _ageGroupRepository;

    public DeleteAgeGroupCommandHandler(IAgeGroupRepository ageGroupRepository)
    {
        _ageGroupRepository = ageGroupRepository;
    }

    public async Task<bool> Handle(DeleteAgeGroupCommand request, CancellationToken cancellationToken)
    {
        var ageGroup = await _ageGroupRepository.GetByIdAsync(request.Id, cancellationToken);
        if (ageGroup == null)
        {
            return false;
        }

        await _ageGroupRepository.DeleteAsync(ageGroup, cancellationToken);
        return true;
    }
}
