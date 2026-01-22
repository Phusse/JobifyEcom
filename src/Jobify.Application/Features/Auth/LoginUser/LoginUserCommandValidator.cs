using FluentValidation;
using Jobify.Domain.Entities.Users;

namespace Jobify.Application.Features.Auth.LoginUser;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty()
            .MaximumLength(UserLimits.IdentifierIdMaxLength);

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
