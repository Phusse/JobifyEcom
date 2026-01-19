using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Features.Auth.Models;
using Jobify.Application.Models;

namespace Jobify.Application.Features.Auth.RefreshSession;

public record RefreshSessionCommand(
    Guid? SessionId
) : IMessage<OperationResult<SessionResult>>;
