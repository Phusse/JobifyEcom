using Jobify.Api.Extensions.Responses;
using Jobify.Api.Models;
using Jobify.Api.Services;
using Jobify.Application.CQRS.Messaging;
using Jobify.Application.Features.Auth.LoginUser;
using Jobify.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Api.Endpoints.Auth.Handlers;

public static class LoginUserEndpointHandler
{
    public static async Task<IResult> Handle(
        [FromBody] LoginUserRequest request,
        IMediator mediator,
        CookieService cookieService,
        HttpResponse response)
    {
        var result = await mediator.Send(request);

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
