using Jobify.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace Jobify.Api.Authentication;

internal class AuthenticationResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Succeeded)
        {
            await next(context);
            return;
        }

        context.Response.ContentType = "application/json";

        if (authorizeResult.Challenged)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            ApiResponse<object> response = new(
                Success: false,
                MessageId: "AUTH_UNAUTHORIZED",
                Message: "You must be logged in to access this resource.",
                Details: null,
                Data: null
            );

            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        if (authorizeResult.Forbidden)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;

            ApiResponse<object> response = new(
                Success: false,
                MessageId: "AUTH_FORBIDDEN",
                Message: "You do not have permission to access this resource.",
                Details: null,
                Data: null
            );

            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        // fallback to default handler if something unexpected happens
        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
