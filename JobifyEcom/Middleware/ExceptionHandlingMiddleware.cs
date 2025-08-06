using System.Text.Json;
using JobifyEcom.DTOs;
using JobifyEcom.Exceptions;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace JobifyEcom.Middleware;

/// <summary>
/// Middleware for global exception handling in the application.
/// Catches unhandled exceptions and formats them into a consistent <see cref="ApiResponse{T}"/>.
/// </summary>
/// <param name="next">The next middleware component in the pipeline.</param>
/// <param name="logger">Logger used to log exceptions.</param>
/// <param name="env">Provides information about the hosting environment.</param>
/// <param name="jsonOptions">JSON options used to serialize the response.</param>
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env, IOptions<JsonOptions> jsonOptions)
{
	private readonly RequestDelegate _next = next;
	private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;
	private readonly IHostEnvironment _env = env;
	private readonly JsonSerializerOptions _jsonOptions = jsonOptions.Value.SerializerOptions;

	/// <summary>
	/// Invokes the middleware to handle exceptions in the HTTP request pipeline.
	/// </summary>
	/// <param name="context">The current HTTP context.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			await HandleExceptionAsync(context, ex);
		}
	}

	/// <summary>
	/// Handles the caught exception and writes a formatted JSON response.
	/// </summary>
	/// <param name="context">The current HTTP context.</param>
	/// <param name="ex">The exception to handle.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	private async Task HandleExceptionAsync(HttpContext context, Exception ex)
	{
		string traceId = context.TraceIdentifier;
		context.Response.ContentType = "application/json";

		ApiResponse<object?> response;

		if (ex is AppException appEx)
		{
			context.Response.StatusCode = appEx.StatusCode;
			response = ApiResponse<object?>.Fail(null, appEx.Message, appEx.Errors, traceId);
		}
		else
		{
			context.Response.StatusCode = StatusCodes.Status500InternalServerError;

			string message = _env.IsDevelopment()
				? ex.InnerException?.Message ?? ex.Message
				: "Something went wrong.";

			List<string> errors = [message];

			_logger.LogError(ex, "Unhandled exception occurred. TraceId: {TraceId}", traceId);
			response = ApiResponse<object?>.Fail(null, message, errors, traceId);
		}

		string json = JsonSerializer.Serialize(response, _jsonOptions);
		await context.Response.WriteAsync(json);
	}
}
