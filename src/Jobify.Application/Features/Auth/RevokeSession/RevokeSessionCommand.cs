using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Models;

namespace Jobify.Application.Features.Auth.RevokeSession;

public record RevokeSessionCommand(
    Guid? SessionId
) : IMessage<OperationResult<object>>;
