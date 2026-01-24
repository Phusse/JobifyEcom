using FluentValidation;
using Jobify.Ecom.Domain.Entities.Jobs;

namespace Jobify.Ecom.Application.Features.Jobs.CreateJob;

public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
    public CreateJobCommandValidator()
    {
        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(JobLimits.TitleMinLength)
            .MaximumLength(JobLimits.TitleMaxLength);

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(JobLimits.DescriptionMinLength)
            .MaximumLength(JobLimits.DescriptionMaxLength);

        RuleFor(x => x.MinSalary)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(JobLimits.MinAllowedMoney)
            .LessThanOrEqualTo(JobLimits.MaxAllowedMoney);

        RuleFor(x => x.MaxSalary)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(JobLimits.MinAllowedMoney)
            .LessThanOrEqualTo(JobLimits.MaxAllowedMoney)
            .GreaterThanOrEqualTo(x => x.MinSalary)
            .WithMessage("Maximum salary must be greater than or equal to minimum salary.");

        RuleFor(x => x.ClosingDate)
            .Must(BeInTheFuture)
            .WithMessage("Closing date must be in the future.");
    }

    private static bool BeInTheFuture(DateTime closingDate)
        => closingDate > DateTime.UtcNow;
}
