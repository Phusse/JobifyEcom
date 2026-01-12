using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Features.Auth.Models;
using Jobify.Application.Models;

namespace Jobify.Application.Features.Auth.LoginUser;

public record LoginUserRequest(
    string Identifier,
    string Password,
    bool RememberMe = false
) : IRequest<OperationResult<SessionResult>>;
