using Jobify.Application.Constants.Responses;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Extensions.Responses;
using Jobify.Application.Features.Auth.Extensions;
using Jobify.Application.Features.Auth.Models;
using Jobify.Application.Models;
using Jobify.Application.Services;

namespace Jobify.Application.Features.Auth.RefreshSession;

public class RefreshSessionHandler(SessionManagementService sessionService)
    : IHandler<RefreshSessionRequest, OperationResult<SessionResult>>
{
    public async Task<OperationResult<SessionResult>> Handle(RefreshSessionRequest message, CancellationToken cancellationToken = default)
    {
        if (message.SessionId is null)
            throw ResponseCatalog.Auth.InvalidSession.ToException();

        SessionData sessionData = await sessionService
            .ExtendSessionAsync(message.SessionId.Value, cancellationToken)
            ?? throw ResponseCatalog.Auth.InvalidSession.ToException();

        SessionTimestampsResponse timeStamps = sessionData.ToTimestampsResponse();
        SessionResult data = new(sessionData.SessionId, timeStamps);

        return ResponseCatalog.Auth.SessionRefreshed
            .As<SessionResult>()
            .WithData(data)
            .ToOperationResult();
    }
}
