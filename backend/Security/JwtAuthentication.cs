using JobifyEcom.Contracts.Errors;
using JobifyEcom.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace JobifyEcom.Security;

/// <summary>
/// Provides internal helper methods for handling JWT authentication events.
/// </summary>
internal static class JwtAuthentication
{
	/// <summary>
	/// Handles JWT authentication failures.
	/// </summary>
	/// <remarks>
	/// This method is intended to be assigned to the <see cref="JwtBearerEvents.OnAuthenticationFailed"/> event.
	/// It checks if the current endpoint requires authorization. If it does, an <see cref="AppException"/>
	/// with an "Unauthorized" error is thrown. Otherwise, the error is suppressed so unauthenticated requests
	/// can still access public endpoints.
	/// </remarks>
	/// <param name="context">The <see cref="AuthenticationFailedContext"/> containing information about the failed authentication.</param>
	/// <returns>A <see cref="Task"/> that completes when the failure handling is finished.</returns>
	internal static Task HandleAuthenticationFailedAsync(AuthenticationFailedContext context)
	{
		// Check if the endpoint has an [Authorize] attribute
		Endpoint? endpoint = context.HttpContext.GetEndpoint();
		bool hasAuthorize = endpoint?.Metadata?.GetMetadata<IAuthorizeData>() is not null;

		if (hasAuthorize)
		{
			throw new AppException(ErrorCatalog.Unauthorized);
		}

		// Endpoint is public; suppress the error so the request can continue
		context.NoResult();
		return Task.CompletedTask;
	}
}
