using Jobify.Api.Constants.Cookies;
using Jobify.Api.Extensions.Responses;
using Jobify.Api.Services;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Features.Users.GetCurrentUser;
using Jobify.Application.Features.Users.Models;
using Jobify.Application.Models;

namespace Jobify.Api.Endpoints.Users.Handlers;

public static class GetCurrentUserEndpointHandler
{
    public static async Task<IResult> Handle(HttpRequest request, IMediator mediator, CookieService cookieService)
    {
        string? sessionIdStr = cookieService.GetCookie(request, CookieKeys.Session);
        Guid? sessionId = null;

        if (!string.IsNullOrEmpty(sessionIdStr) && Guid.TryParseExact(sessionIdStr, "N", out Guid parsed))
            sessionId = parsed;

        OperationResult<UserResponse> result = await mediator.Send(new GetCurrentUserRequest(sessionId));

        return Results.Ok(result.ToApiResponse());
    }
}
