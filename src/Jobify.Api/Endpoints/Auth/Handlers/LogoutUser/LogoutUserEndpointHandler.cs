using System.Security.Claims;
using Jobify.Api.Constants.Auth;
using Jobify.Api.Constants.Cookies;
using Jobify.Api.Extensions.Responses;
using Jobify.Api.Helpers;
using Jobify.Api.Models;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Features.Auth.Logout;
using Jobify.Application.Models;

namespace Jobify.Api.Endpoints.Auth.Handlers.LogoutUser;

internal static class LogoutUserEndpointHandler
{
    public static async Task<IResult> Handle(HttpContext context, IMediator mediator)
    {
        Guid? sessionId = null;
        string? rawSessionId = context.User.FindFirstValue(SessionClaimTypes.SessionId);

        if (Guid.TryParse(rawSessionId, out Guid parsedSessionId))
            sessionId = parsedSessionId;

        OperationResult<object> result = await mediator.Send(new LogoutUserCommand(sessionId));

        CookieHelper.DeleteCookie(context.Response, CookieKeys.Session);

        ApiResponse<object> apiResponse = result.ToApiResponse();

        return Results.Ok(apiResponse);
    }
}
