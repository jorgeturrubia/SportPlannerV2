using MediatR;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Application.UseCases.Planning;

public class CreateObjectiveCategoryCommandHandler : IRequestHandler<CreateObjectiveCategoryCommand, Guid>
{
    private readonly IObjectiveCategoryRepository _objectiveCategoryRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateObjectiveCategoryCommandHandler(
        IObjectiveCategoryRepository objectiveCategoryRepository,
        ICurrentUserService currentUserService)
    {
        _objectiveCategoryRepository = objectiveCategoryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateObjectiveCategoryCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        var dto = request.Category;

        // Create objective category
        var category = new ObjectiveCategory(dto.Name, dto.SportId);

        // Set audit fields
        category.CreatedBy = userId.ToString();
        category.CreatedAt = DateTime.UtcNow;

        await _objectiveCategoryRepository.AddAsync(category, cancellationToken);

        return category.Id;
    }
}
