using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Features.Users.Models;
using Jobify.Application.Models;

namespace Jobify.Application.Features.Users.GetCurrentUser;

public record GetCurrentUserQuery(
    Guid? UserId
) : IMessage<OperationResult<UserResponse>>;
