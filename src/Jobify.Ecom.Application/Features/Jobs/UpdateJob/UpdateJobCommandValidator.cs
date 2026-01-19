using FluentValidation;

namespace Jobify.Ecom.Application.Features.Jobs.UpdateJob;

public class UpdateJobCommandValidator : AbstractValidator<UpdateJobCommand>
{
    public UpdateJobCommandValidator()
    {
        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .MinimumLength(1)
            .MaximumLength(150)
            .When(x => !string.IsNullOrWhiteSpace(x.Title));

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .MinimumLength(1)
            .MaximumLength(3000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.MinSalary)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinSalary.HasValue);

        RuleFor(x => x.MaxSalary)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(0)
            .GreaterThanOrEqualTo(x => x.MinSalary ?? 0)
            .WithMessage("Maximum salary must be greater than or equal to minimum salary.")
            .When(x => x.MaxSalary.HasValue);

        RuleFor(x => x.ClosingDate)
            .Must(BeInTheFuture)
            .WithMessage("Closing date must be in the future.")
            .When(x => x.ClosingDate.HasValue);
    }

    private static bool BeInTheFuture(DateTime? closingDate)
        => closingDate.HasValue && closingDate.Value > DateTime.UtcNow;
}
