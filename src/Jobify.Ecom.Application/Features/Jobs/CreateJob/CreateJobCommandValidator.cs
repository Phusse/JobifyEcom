using FluentValidation;

namespace Jobify.Ecom.Application.Features.Jobs.CreateJob;

public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
    public CreateJobCommandValidator()
    {
        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(3000);

        RuleFor(x => x.MinSalary)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.MaxSalary)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(0)
            .GreaterThanOrEqualTo(x => x.MinSalary)
            .WithMessage("Maximum salary must be greater than or equal to minimum salary.");

        RuleFor(x => x.ClosingDate)
            .Must(BeInTheFuture)
            .WithMessage("Closing date must be in the future.");
    }

    private static bool BeInTheFuture(DateTime closingDate)
        => closingDate > DateTime.UtcNow;
}
