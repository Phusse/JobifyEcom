using Jobify.Api.Constants.Cookies;
using Jobify.Api.Helpers;
using Jobify.Api.Models;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Features.Auth.LoginUser;
using Jobify.Application.Features.Auth.Models;
using Jobify.Application.Features.Auth.RevokeSession;
using Jobify.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Api.Endpoints.Auth.Handlers.LoginUser;

internal static class LoginUserEndpointHandler
{
    public static async Task<IResult> Handle([FromBody] LoginUserCommand message, IMediator mediator, HttpResponse response, HttpRequest request)
    {
        OperationResult<SessionResult> result = await mediator.Send(message);

        Guid? sessionId = null;
        string? rawSessionId = CookieHelper.GetCookie(request, CookieKeys.Session);

        if (Guid.TryParse(rawSessionId, out Guid parsedSessionId))
            sessionId = parsedSessionId;

        if (sessionId.HasValue)
            await mediator.Send(new RevokeSessionCommand(sessionId));

        SessionResult data = result.Data!;

        CookieHelper.SetCookie(
            response,
            CookieKeys.Session,
            data.SessionId.ToString("N"),
            expiresUtc: data.Timestamps.ExpiresAt
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
