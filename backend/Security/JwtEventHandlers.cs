using JobifyEcom.Contracts.Errors;
using JobifyEcom.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace JobifyEcom.Security;

/// <summary>
/// Factory class for creating <see cref="JwtBearerEvents"/> instances.
/// This class wires up token validation, challenge, and forbidden event handlers,
/// delegating actual logic to dedicated helper classes.
/// </summary>
internal static class JwtEventHandlers
{
	/// <summary>
	/// Creates and configures a <see cref="JwtBearerEvents"/> instance.
	/// </summary>
	/// <returns>
	/// A fully configured <see cref="JwtBearerEvents"/> instance that performs
	/// token validation and more.
	/// </returns>
	internal static JwtBearerEvents Create()
	{
		return new JwtBearerEvents
		{
			OnTokenValidated = TokenValidator.ValidateAsync,
			OnAuthenticationFailed = context =>
			{
				// throw new AppException(ErrorCatalog.Unauthorized);

				// If the endpoint does NOT require [Authorize], ignore auth failures.
				var endpoint = context.HttpContext.GetEndpoint();
				var hasAuthorize = endpoint?.Metadata?.GetMetadata<IAuthorizeData>() is not null;

				if (hasAuthorize)
				{
					throw new AppException(ErrorCatalog.Unauthorized);
				}

				// Suppress the error so unauthenticated requests still hit public endpoints
				context.NoResult();
				return Task.CompletedTask;
			},
			OnForbidden = context =>
			{
				throw new AppException(ErrorCatalog.Forbidden);
			},
		};
	}
}
