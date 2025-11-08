using FluentValidation;

namespace SportPlanner.Application.UseCases.Planning;

public class CreateObjectiveCategoryCommandValidator : AbstractValidator<CreateObjectiveCategoryCommand>
{
    public CreateObjectiveCategoryCommandValidator()
    {
        RuleFor(x => x.Category.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(200)
            .WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Category.SportId)
            .IsInEnum()
            .WithMessage("Sport must be a valid value");
    }
}
