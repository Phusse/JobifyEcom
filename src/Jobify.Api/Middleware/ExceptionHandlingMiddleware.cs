using System.Text.Json;
using Jobify.Api.Extensions.Responses;
using Jobify.Api.Models;
using Jobify.Application.Exceptions;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace Jobify.Api.Middleware;

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
        ApiResponse<object> response;
        string traceId = httpContext.TraceIdentifier;

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
