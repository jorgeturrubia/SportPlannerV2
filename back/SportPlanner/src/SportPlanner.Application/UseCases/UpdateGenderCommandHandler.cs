using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases;

public class UpdateGenderCommandHandler : IRequestHandler<UpdateGenderCommand, GenderResponse>
{
    private readonly IGenderRepository _genderRepository;

    public UpdateGenderCommandHandler(IGenderRepository genderRepository)
    {
        _genderRepository = genderRepository;
    }

    public async Task<GenderResponse> Handle(UpdateGenderCommand request, CancellationToken cancellationToken)
    {
        var gender = await _genderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (gender == null)
        {
            throw new KeyNotFoundException($"Gender with ID {request.Id} not found");
        }

        gender.UpdateDetails(request.Name, request.Description);

        if (request.IsActive)
            gender.Activate();
        else
            gender.Deactivate();

        await _genderRepository.UpdateAsync(gender, cancellationToken);

        return new GenderResponse(
            gender.Id,
            gender.Name,
            gender.Code,
            gender.Description,
            gender.IsActive
        );
    }
}
