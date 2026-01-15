using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.Users.Models;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Application.Features.Users.GetCurrentUser;

public record GetCurrentUserRequest(
    Guid? UserId
) : IRequest<OperationResult<UserResponse>>;
