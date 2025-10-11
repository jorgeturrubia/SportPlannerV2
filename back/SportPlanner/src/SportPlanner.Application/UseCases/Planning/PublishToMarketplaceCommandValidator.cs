using FluentValidation;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.UseCases.Planning;

public class PublishToMarketplaceCommandValidator : AbstractValidator<PublishToMarketplaceCommand>
{
    public PublishToMarketplaceCommandValidator()
    {
        RuleFor(x => x.SourceEntityId)
            .NotEmpty()
            .WithMessage("Source entity ID cannot be empty.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("A valid item type must be specified.");
    }
}