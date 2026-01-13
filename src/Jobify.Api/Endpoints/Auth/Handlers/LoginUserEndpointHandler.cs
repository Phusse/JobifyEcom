using Jobify.Api.Constants.Cookies;
using Jobify.Api.Models;
using Jobify.Api.Services;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Features.Auth.LoginUser;
using Jobify.Application.Features.Auth.Models;
using Jobify.Application.Features.Auth.RevokeSession;
using Jobify.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Api.Endpoints.Auth.Handlers;

public static class LoginUserEndpointHandler
{
    public static async Task<IResult> Handle([FromBody] LoginUserRequest message, IMediator mediator, CookieService cookieService, HttpResponse response, HttpRequest request)
    {
        OperationResult<SessionResult> result = await mediator.Send(message);

        Guid? sessionId = null;
        string? rawSessionId = cookieService.GetCookie(request, CookieKeys.Session);

        if (Guid.TryParse(rawSessionId, out Guid parsedSessionId))
            sessionId = parsedSessionId;

        if (sessionId.HasValue)
            await mediator.Send(new RevokeSessionRequest(sessionId));

        SessionResult data = result.Data!;

        cookieService.SetCookie(
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
