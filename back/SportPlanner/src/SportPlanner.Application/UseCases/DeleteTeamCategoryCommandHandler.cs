using MediatR;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases;

public class DeleteTeamCategoryCommandHandler : IRequestHandler<DeleteTeamCategoryCommand, bool>
{
    private readonly ITeamCategoryRepository _teamCategoryRepository;

    public DeleteTeamCategoryCommandHandler(ITeamCategoryRepository teamCategoryRepository)
    {
        _teamCategoryRepository = teamCategoryRepository;
    }

    public async Task<bool> Handle(DeleteTeamCategoryCommand request, CancellationToken cancellationToken)
    {
        var teamCategory = await _teamCategoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (teamCategory == null)
        {
            return false;
        }

        await _teamCategoryRepository.DeleteAsync(teamCategory, cancellationToken);
        return true;
    }
}
