using Jobify.Api.Constants.Cookies;
using Jobify.Api.Helpers;
using Jobify.Application.Services;

namespace Jobify.Api.Middleware;

internal class SessionRefreshMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, SessionManagementService sessionManagementService)
    {
        try
        {
            await TryRefreshSessionAsync(context, sessionManagementService, context.RequestAborted);
        }
        catch { }

        await next(context);
    }

    private static async Task TryRefreshSessionAsync(HttpContext context, SessionManagementService sessionManagementService, CancellationToken cancellationToken = default)
    {
        string? rawSessionId = CookieHelper.GetCookie(context.Request, CookieKeys.Session);

        if (!Guid.TryParse(rawSessionId, out Guid sessionId))
            return;

        await sessionManagementService.ExtendSessionAsync(sessionId, cancellationToken);
    }
}
