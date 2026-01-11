using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Features.Users.Models;
using Jobify.Application.Models;

namespace Jobify.Application.Features.Users.GetCurrentUser;

public record GetCurrentUserRequest(
    Guid? SessionId
) : IRequest<OperationResult<UserResponse>>;
