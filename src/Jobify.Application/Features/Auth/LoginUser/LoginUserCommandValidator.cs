using FluentValidation;

namespace Jobify.Application.Features.Auth.LoginUser;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty()
            .MaximumLength(254);

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
