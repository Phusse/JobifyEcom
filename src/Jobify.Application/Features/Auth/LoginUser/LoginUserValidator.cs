using FluentValidation;

namespace Jobify.Application.Features.Auth.LoginUser;

public class LoginUserValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty()
            .MaximumLength(254);

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
