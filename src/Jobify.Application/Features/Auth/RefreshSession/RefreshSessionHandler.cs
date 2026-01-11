using Jobify.Application.Constants.Responses;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Extensions.Responses;
using Jobify.Application.Features.Auth.Extensions;
using Jobify.Application.Features.Auth.Models;
using Jobify.Application.Models;
using Jobify.Application.Services;

namespace Jobify.Application.Features.Auth.RefreshSession;

public class RefreshSessionHandler(SessionManagementService sessionService)
    : IHandler<RefreshSessionRequest, OperationResult<SessionTimestampsResponse>>
{
    public async Task<OperationResult<SessionTimestampsResponse>> Handle(RefreshSessionRequest message, CancellationToken cancellationToken = default)
    {
        if (message.SessionId is null)
            throw ResponseCatalog.Auth.InvalidSession.ToException();

        SessionData sessionData = await sessionService.RefreshSessionAsync(message.SessionId.Value, cancellationToken)
            ?? throw ResponseCatalog.Auth.InvalidSession.ToException();

        SessionTimestampsResponse data = sessionData.ToTimestampsResponse();

        return ResponseCatalog.Auth.SessionRefreshed
            .As<SessionTimestampsResponse>()
            .WithData(data)
            .ToOperationResult();
    }
}
