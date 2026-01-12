using Jobify.Api.Constants.Cookies;
using Jobify.Application.Services;

namespace Jobify.Api.Middleware;

internal class SessionRefreshMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, SessionManagementService sessionManagementService)
    {
        try
        {
            await TryRefreshSessionAsync(context, sessionManagementService, context.RequestAborted);
        }
        catch { }

        await _next(context);
    }

    private static async Task TryRefreshSessionAsync(HttpContext context, SessionManagementService sessionManagementService, CancellationToken cancellationToken)
    {
        if (!context.Request.Cookies.TryGetValue(CookieKeys.Session, out string? rawSessionId))
            return;

        if (!Guid.TryParse(rawSessionId, out Guid sessionId))
            return;

        await sessionManagementService.ExtendSessionAsync(sessionId, cancellationToken);
    }
}
