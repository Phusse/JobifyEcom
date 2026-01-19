namespace Jobify.Api.Middleware;

internal class TraceIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        string traceId = context.TraceIdentifier;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-Trace-Id"] = traceId;
            return Task.CompletedTask;
        });

        await next(context);
    }
}
