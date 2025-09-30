using FluentValidation;

namespace SportPlanner.Application.UseCases.Planning;

public class UpdateObjectiveCommandValidator : AbstractValidator<UpdateObjectiveCommand>
{
    public UpdateObjectiveCommandValidator()
    {
        RuleFor(x => x.Objective.Id)
            .NotEmpty().WithMessage("Objective ID is required");

        RuleFor(x => x.Objective.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Objective.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.Objective.ObjectiveCategoryId)
            .NotEmpty().WithMessage("Category is required");

        RuleFor(x => x.Objective.Techniques)
            .NotNull().WithMessage("Techniques list cannot be null");

        RuleForEach(x => x.Objective.Techniques).ChildRules(technique =>
        {
            technique.RuleFor(t => t.Description)
                .NotEmpty().WithMessage("Technique description is required")
                .MaximumLength(500).WithMessage("Technique description must not exceed 500 characters");

            technique.RuleFor(t => t.Order)
                .GreaterThanOrEqualTo(0).WithMessage("Technique order must be non-negative");
        });
    }
}