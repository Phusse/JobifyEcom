using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Models;

namespace Jobify.Application.Features.Auth.RevokeSession;

public record RevokeSessionRequest(
    Guid? SessionId
) : IRequest<OperationResult<object>>;
