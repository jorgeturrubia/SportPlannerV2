using MediatR;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases;

public class DeleteGenderCommandHandler : IRequestHandler<DeleteGenderCommand, bool>
{
    private readonly IGenderRepository _genderRepository;

    public DeleteGenderCommandHandler(IGenderRepository genderRepository)
    {
        _genderRepository = genderRepository;
    }

    public async Task<bool> Handle(DeleteGenderCommand request, CancellationToken cancellationToken)
    {
        var gender = await _genderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (gender == null)
        {
            return false;
        }

        await _genderRepository.DeleteAsync(gender, cancellationToken);
        return true;
    }
}
