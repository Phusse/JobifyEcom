using FluentValidation;

namespace Jobify.Application.Features.Auth.RegisterUser;

public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.FirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(40)
            .Matches("^[a-zA-Z'-]+$").WithMessage("Names can only contain letters, hyphens, or apostrophes.");

        RuleFor(x => x.MiddleName)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(40)
            .Matches("^[a-zA-Z'-]+$").WithMessage("Names can only contain letters, hyphens, or apostrophes.");

        RuleFor(x => x.LastName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(60)
            .Matches("^[a-zA-Z'-]+$").WithMessage("Names can only contain letters, hyphens, or apostrophes.");

        RuleFor(x => x.UserName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(30)
            .Matches("^[a-zA-Z0-9._]+$").WithMessage("Username can only contain letters, numbers, dots, or underscores.");

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(254)
            .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"^\S+$").WithMessage("Password cannot contain spaces.");

        When(x => !string.IsNullOrEmpty(x.Password), () =>
        {
            RuleFor(x => x.Password)
                .Must(ContainUppercase).WithMessage("Password must contain at least one uppercase letter.")
                .Must(ContainLowercase).WithMessage("Password must contain at least one lowercase letter.")
                .Must(ContainDigit).WithMessage("Password must contain at least one number.")
                .Must(ContainSymbol).WithMessage("Password must contain at least one special character.");
        });
    }

    private bool ContainUppercase(string password) => password.Any(char.IsUpper);
    private bool ContainLowercase(string password) => password.Any(char.IsLower);
    private bool ContainDigit(string password) => password.Any(char.IsDigit);
    private bool ContainSymbol(string password) => password.Any(c => !char.IsLetterOrDigit(c));
}
