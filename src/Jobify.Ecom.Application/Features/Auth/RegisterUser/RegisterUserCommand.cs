using Jobify.Ecom.Application.CQRS.Messaging;
using Jobify.Ecom.Application.Models;

namespace Jobify.Ecom.Application.Features.Auth.RegisterUser;

public record RegisterUserCommand(
    Guid? SourceUserId
) : IMessage<OperationResult<Guid>>;
