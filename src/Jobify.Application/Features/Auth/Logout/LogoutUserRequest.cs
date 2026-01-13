using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Models;

namespace Jobify.Application.Features.Auth.Logout;

public record LogoutUserRequest(
    Guid? SessionId
) : IRequest<OperationResult<object>>;
