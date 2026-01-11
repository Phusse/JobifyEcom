using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Features.Users.Models;
using Jobify.Application.Models;

namespace Jobify.Application.Features.Users.GetUserById;

public record GetUserByIdRequest(
    Guid Id
) : IRequest<OperationResult<UserResponse>>;
