using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Features.Auth.Models;
using Jobify.Application.Models;

namespace Jobify.Application.Features.Auth.LoginUser;

public record LoginUserCommand(
    string Identifier,
    string Password,
    bool RememberMe = false
) : IMessage<OperationResult<SessionResult>>;
