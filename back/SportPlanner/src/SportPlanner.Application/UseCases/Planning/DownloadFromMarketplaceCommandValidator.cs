using FluentValidation;

namespace SportPlanner.Application.UseCases.Planning;

public class DownloadFromMarketplaceCommandValidator : AbstractValidator<DownloadFromMarketplaceCommand>
{
    public DownloadFromMarketplaceCommandValidator()
    {
        RuleFor(x => x.MarketplaceItemId)
            .NotEmpty()
            .WithMessage("Marketplace Item ID cannot be empty.");
    }
}