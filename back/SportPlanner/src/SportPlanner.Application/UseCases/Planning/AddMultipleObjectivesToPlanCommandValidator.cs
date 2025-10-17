using FluentValidation;

namespace SportPlanner.Application.UseCases.Planning;

/// <summary>
/// Validator for batch adding objectives to a training plan
/// </summary>
public class AddMultipleObjectivesToPlanCommandValidator : AbstractValidator<AddMultipleObjectivesToPlanCommand>
{
    public AddMultipleObjectivesToPlanCommandValidator()
    {
        RuleFor(x => x.TrainingPlanId)
            .NotEmpty()
            .WithMessage("Training plan ID is required");

        RuleFor(x => x.Objectives)
            .NotEmpty()
            .WithMessage("At least one objective is required")
            .Must(x => x.Count <= 100)
            .WithMessage("Cannot add more than 100 objectives at once");

        RuleForEach(x => x.Objectives)
            .SetValidator(new AddObjectiveBatchItemValidator());
    }
}

/// <summary>
/// Nested validator for each objective in the batch
/// </summary>
public class AddObjectiveBatchItemValidator : AbstractValidator<AddObjectiveBatchItem>
{
    public AddObjectiveBatchItemValidator()
    {
        RuleFor(o => o.ObjectiveId)
            .NotEmpty()
            .WithMessage("Objective ID is required");

        RuleFor(o => o.Priority)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(5)
            .WithMessage("Priority must be between 1 and 5");

        RuleFor(o => o.TargetSessions)
            .GreaterThan(0)
            .WithMessage("Target sessions must be greater than 0")
            .LessThan(1000)
            .WithMessage("Target sessions cannot exceed 999");
    }
}
