using System.Text.Json;
using JobifyEcom.Contracts.Errors;
using JobifyEcom.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
	/// <param name="jsonOptions">
	/// The JSON serialization options to use when formatting error responses.
	/// </param>
	/// <returns>
	/// A fully configured <see cref="JwtBearerEvents"/> instance that performs
	/// token validation and more.
	/// </returns>
	internal static JwtBearerEvents Create(JsonSerializerOptions jsonOptions)
	{
		return new JwtBearerEvents
		{
			OnTokenValidated = TokenValidator.ValidateAsync,
			OnForbidden = context =>
			{
				throw new AppException(ErrorCatalog.Forbidden);
			}
		};
	}
}
