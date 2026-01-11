using Jobify.Api.Extensions.Responses;
using Jobify.Api.Models;
using Jobify.Api.Services;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Features.Auth.Logout;
using Jobify.Application.Models;

namespace Jobify.Api.Endpoints.Auth.Handlers;

public static class LogoutUserEndpointHandler
{
    public static async Task<IResult> Handle(
        HttpRequest request,
        IMediator mediator,
        CookieService cookieService,
        HttpResponse response)
    {
        string? sessionIdStr = cookieService.GetCookie(request, "X-Session-Id");

        Guid sessionId = Guid.Empty;
        if (!string.IsNullOrEmpty(sessionIdStr))
        {
            Guid.TryParse(sessionIdStr, out sessionId);
        }

        OperationResult<object?> result = await mediator.Send(new LogoutUserRequest(sessionId));

        cookieService.DeleteCookie(response, "X-Session-Id");

        ApiResponse<object?> apiResponse = result.ToApiResponse();

        return Results.Ok(apiResponse);
    }
}
