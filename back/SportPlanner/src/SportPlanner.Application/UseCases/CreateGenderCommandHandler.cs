using MediatR;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities;

namespace SportPlanner.Application.UseCases;

public class CreateGenderCommandHandler : IRequestHandler<CreateGenderCommand, GenderResponse>
{
    private readonly IGenderRepository _genderRepository;

    public CreateGenderCommandHandler(IGenderRepository genderRepository)
    {
        _genderRepository = genderRepository;
    }

    public async Task<GenderResponse> Handle(CreateGenderCommand request, CancellationToken cancellationToken)
    {
        var gender = new Gender(request.Name, request.Code, request.Description);

        if (!request.IsActive)
            gender.Deactivate();

        await _genderRepository.AddAsync(gender, cancellationToken);

        return new GenderResponse(
            gender.Id,
            gender.Name,
            gender.Code,
            gender.Description,
            gender.IsActive
        );
    }
}
