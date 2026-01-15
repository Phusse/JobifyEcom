using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Features.Users.Models;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Application.Features.Users.GetUserById;

public record GetUserByIdRequest(
    Guid Id
) : IRequest<OperationResult<UserResponse>>;
