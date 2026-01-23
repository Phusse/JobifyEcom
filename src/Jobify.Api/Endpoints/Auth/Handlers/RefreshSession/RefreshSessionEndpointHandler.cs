using System.Security.Claims;
using Jobify.Api.Constants.Auth;
using Jobify.Api.Constants.Cookies;
using Jobify.Api.Helpers;
using Jobify.Api.Models;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Features.Auth.Models;
using Jobify.Application.Features.Auth.RefreshSession;
using Jobify.Application.Models;

namespace Jobify.Api.Endpoints.Auth.Handlers.RefreshSession;

internal static class RefreshSessionEndpointHandler
{
    public static async Task<IResult> Handle(HttpContext context, IMediator mediator)
    {
        Guid? sessionId = null;
        string? rawSessionId = context.User.FindFirstValue(SessionClaimTypes.SessionId);

        if (Guid.TryParse(rawSessionId, out Guid parsedSessionId))
            sessionId = parsedSessionId;

        OperationResult<SessionResult> result = await mediator.Send(new RefreshSessionCommand(sessionId));

        SessionResult data = result.Data!;

        CookieHelper.SetCookie(
            context.Response,
            CookieKeys.Session,
            data.SessionId.ToString("N"),
            data.Timestamps.ExpiresAt
        );

        ApiResponse<SessionTimestampsResponse> apiResponse = new(
            Success: true,
            MessageId: result.MessageId,
            Message: result.Message,
            Details: result.Details,
            Data: data.Timestamps
        );

        return Results.Ok(apiResponse);
    }
}
