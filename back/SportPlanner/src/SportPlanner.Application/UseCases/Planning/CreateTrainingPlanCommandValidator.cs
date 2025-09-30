using FluentValidation;

namespace SportPlanner.Application.UseCases.Planning;

public class CreateTrainingPlanCommandValidator : AbstractValidator<CreateTrainingPlanCommand>
{
    public CreateTrainingPlanCommandValidator()
    {
        RuleFor(x => x.TrainingPlan.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.TrainingPlan.StartDate)
            .NotEmpty().WithMessage("Start date is required");

        RuleFor(x => x.TrainingPlan.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThan(x => x.TrainingPlan.StartDate).WithMessage("End date must be after start date");

        RuleFor(x => x.TrainingPlan.Schedule)
            .NotNull().WithMessage("Schedule is required");

        RuleFor(x => x.TrainingPlan.Schedule.TrainingDays)
            .NotEmpty().WithMessage("Training days are required")
            .When(x => x.TrainingPlan.Schedule != null);

        RuleFor(x => x.TrainingPlan.Schedule.TotalWeeks)
            .GreaterThan(0).WithMessage("Total weeks must be greater than zero")
            .When(x => x.TrainingPlan.Schedule != null);

        RuleFor(x => x.TrainingPlan.Objectives)
            .NotNull().WithMessage("Objectives list cannot be null");

        RuleForEach(x => x.TrainingPlan.Objectives).ChildRules(objective =>
        {
            objective.RuleFor(o => o.ObjectiveId)
                .NotEmpty().WithMessage("Objective ID is required");

            objective.RuleFor(o => o.Priority)
                .InclusiveBetween(1, 5).WithMessage("Priority must be between 1 and 5");

            objective.RuleFor(o => o.TargetSessions)
                .GreaterThan(0).WithMessage("Target sessions must be greater than zero");
        });
    }
}