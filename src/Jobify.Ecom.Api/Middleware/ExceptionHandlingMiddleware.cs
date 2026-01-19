using System.Text.Json;
using Jobify.Ecom.Api.Extensions.Responses;
using Jobify.Ecom.Api.Models;
using Jobify.Ecom.Application.Exceptions;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Jobify.Ecom.Api.Middleware;

internal class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IOptions<JsonOptions> jsonOptions)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
    {
        string traceId = httpContext.Request.Headers
            .TryGetValue("X-Trace-Id", out StringValues values) && !string.IsNullOrEmpty(values)
            ? values.ToString()
            : httpContext.TraceIdentifier;

        ApiResponse<object> response;

        if (ex is AppException appEx)
        {
            httpContext.Response.StatusCode = appEx.StatusCode;
            response = appEx.ToApiResponse<object>();

            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning(
                    "Handled application error | TraceId: {TraceId} | MessageId: {MessageId}",
                    traceId,
                    response.MessageId
                );
        }
        else
        {
            httpContext.Response.StatusCode = 500;
            response = ex.ToApiResponse<object>();

            if (logger.IsEnabled(LogLevel.Error))
                logger.LogError(
                    ex,
                    "Unhandled server exception | TraceId: {TraceId} | MessageId: {MessageId}",
                    traceId,
                    response.MessageId
                );
        }

        httpContext.Response.ContentType = "application/json; charset=utf-8";
        string json = JsonSerializer.Serialize(response, jsonOptions.Value.SerializerOptions);
        await httpContext.Response.WriteAsync(json);
    }
}
