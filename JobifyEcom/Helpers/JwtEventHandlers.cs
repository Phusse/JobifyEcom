using System.Security.Claims;
using System.Text.Json;
using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Helpers;

/// <summary>
/// Provides factory methods for configuring JWT authentication event handlers,
/// including token claim validation and consistent error responses.
/// </summary>
public static class JwtEventHandlers
{
	private const string AuthErrorKey = "AuthError";

	/// <summary>
	/// Creates a <see cref="JwtBearerEvents"/> instance configured with:
	/// <list type="bullet">
	/// <item>Token claim validation with security stamp checks.</item>
	/// <item>Consistent JSON responses for <c>401 Unauthorized</c> and <c>403 Forbidden</c>.</item>
	/// </list>
	/// </summary>
	/// <param name="jsonOptions">The JSON serialization options used for formatting responses.</param>
	/// <returns>A configured <see cref="JwtBearerEvents"/> instance.</returns>
	public static JwtBearerEvents Create(JsonSerializerOptions jsonOptions)
	{
		return new JwtBearerEvents
		{
			OnTokenValidated = HandleTokenValidatedAsync,
			OnChallenge = context => HandleChallengeAsync(context, jsonOptions),
			OnForbidden = context => HandleForbiddenAsync(context, jsonOptions)
		};
	}

	#region Event Handlers

	private static async Task HandleTokenValidatedAsync(TokenValidatedContext context)
	{
		GetClaims(context.Principal, out string? userIdClaim, out string? securityStampClaim, out string? tokenType);

		if (!string.Equals(tokenType, TokenType.Access.ToString(), StringComparison.OrdinalIgnoreCase))
		{
			FailWithReason(context, "Your session token isn't valid for this action. Please log in again to get the correct token.");
			return;
		}

		if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(securityStampClaim))
		{
			FailWithReason(context, "Your login session could not be verified. Please sign in again.");
			return;
		}

		if (!Guid.TryParse(userIdClaim, out Guid userId))
		{
			FailWithReason(context, "We couldn't confirm your account details. Please sign in again.");
			return;
		}

		var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
		Guid? dbSecurityStamp = await GetUserSecurityStampAsync(db, userId);

		if (dbSecurityStamp is null)
		{
			FailWithReason(context, "This account no longer exists or has been removed.");
			return;
		}

		if (dbSecurityStamp.Value == Guid.Empty)
		{
			FailWithReason(context, "You've been signed out. Please log in again to continue.");
			return;
		}

		if (!string.Equals(dbSecurityStamp.Value.ToString(), securityStampClaim, StringComparison.Ordinal))
		{
			FailWithReason(context, "Your login session is no longer valid. Please sign in again.");
		}
	}

	private static Task HandleChallengeAsync(JwtBearerChallengeContext context, JsonSerializerOptions jsonOptions)
	{
		context.HandleResponse();

		return WriteJsonErrorAsync(
			context.Response,
			StatusCodes.Status401Unauthorized,
			"You need to be signed in to access this feature.",
			context.HttpContext.Items[AuthErrorKey] is not string message ? null : [message],
			jsonOptions
		);
	}

	private static Task HandleForbiddenAsync(ForbiddenContext context, JsonSerializerOptions jsonOptions)
	{
		return WriteJsonErrorAsync(
			context.Response,
			StatusCodes.Status403Forbidden,
			"You need to be signed in to access this feature.",
			["You don't have permission to view this page or perform this action."],
			jsonOptions
		);
	}

	#endregion

	#region Helpers

	private static void GetClaims(ClaimsPrincipal? principal, out string? userId, out string? securityStamp, out string? tokenType)
	{
		userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		securityStamp = principal?.FindFirst("security_stamp")?.Value;
		tokenType = principal?.FindFirst("token_type")?.Value;
	}

	private static async Task<Guid?> GetUserSecurityStampAsync(AppDbContext db, Guid userId)
	{
		return await db.Users.AsNoTracking()
			.Where(u => u.Id == userId)
			.Select(u => (Guid?)u.SecurityStamp)
			.FirstOrDefaultAsync();
	}

	private static void FailWithReason(TokenValidatedContext context, string reason)
	{
		context.HttpContext.Items[AuthErrorKey] = reason;
		context.Fail(reason);
	}

	private static Task WriteJsonErrorAsync(HttpResponse response, int statusCode, string? message, List<string>? errors, JsonSerializerOptions options)
	{
		response.StatusCode = statusCode;
		response.ContentType = "application/json";

		var payload = ApiResponse<object>.Fail(null, message, errors);
		string json = JsonSerializer.Serialize(payload, options);

		return response.WriteAsync(json);
	}

	#endregion
}
