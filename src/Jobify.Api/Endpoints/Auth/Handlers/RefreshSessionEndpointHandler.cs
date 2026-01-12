using System.Security.Claims;
using Jobify.Api.Constants.Auth;
using Jobify.Api.Constants.Cookies;
using Jobify.Api.Models;
using Jobify.Api.Services;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Features.Auth.Models;
using Jobify.Application.Features.Auth.RefreshSession;
using Jobify.Application.Models;

namespace Jobify.Api.Endpoints.Auth.Handlers;

public static class RefreshSessionEndpointHandler
{
    public static async Task<IResult> Handle(HttpContext context, IMediator mediator, CookieService cookieService, HttpResponse response)
    {
        Guid? sessionId = null;
        string? rawSessionId = context.User.FindFirstValue(SessionClaimTypes.SessionId);

        if (Guid.TryParse(rawSessionId, out Guid parsedSessionId))
            sessionId = parsedSessionId;

        OperationResult<SessionResult> result = await mediator.Send(new RefreshSessionRequest(sessionId));

        SessionResult data = result.Data!;

        cookieService.SetCookie(
            response,
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
