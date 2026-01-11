using Jobify.Api.Extensions.Responses;
using Jobify.Api.Models;
using Jobify.Api.Services;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Features.Auth.RefreshSession;
using Jobify.Application.Models;

namespace Jobify.Api.Endpoints.Auth.Handlers;

public static class RefreshSessionEndpointHandler
{
    public static async Task<IResult> Handle(
        HttpRequest request,
        IMediator mediator,
        CookieService cookieService,
        HttpResponse response)
    {
        string? sessionIdStr = cookieService.GetCookie(request, "X-Session-Id");

        if (string.IsNullOrEmpty(sessionIdStr) || !Guid.TryParse(sessionIdStr, out Guid sessionId))
        {
            return Results.Unauthorized();
        }

        var result = await mediator.Send(new RefreshSessionRequest(sessionId));

        var session = result.Data;

        if (session is not null)
        {
            cookieService.SetCookie(
                response,
                "X-Session-Id",
                session.SessionId.ToString(),
                expiresUtc: session.AbsoluteExpiresAt,
                httpOnly: true,
                secure: true
            );
        }

        ApiResponse<object> apiResponse = result.WithoutData()
            .ToApiResponse();

        return Results.Ok(apiResponse);
    }
}
