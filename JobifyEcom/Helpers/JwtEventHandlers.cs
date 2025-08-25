using System.Security.Claims;
using System.Text.Json;
using JobifyEcom.Data;
using JobifyEcom.DTOs;
using JobifyEcom.Extensions;
using JobifyEcom.Models;
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
	private const string AuthMessageKey = "AuthMessage";
	private const string AuthErrorKey = "AuthError";
	private const string AuthStatusCodeKey = "AuthStatusCode";

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
			FailWithReason(
				context,
				"Invalid session",
				"Your session token isn't valid for this action. Please log in again to get the correct token."
			);
			return;
		}

		if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(securityStampClaim))
		{
			FailWithReason(
				context,
				"Session not verified",
				"Your login session could not be verified. Please sign in again."
			);
			return;
		}

		if (!Guid.TryParse(userIdClaim, out Guid userId))
		{
			FailWithReason(
				context,
				"Account not confirmed",
				"We couldn't confirm your account details. Please sign in again."
			);
			return;
		}

		var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

		User? user = await db.Users.AsNoTracking()
			.Include(u => u.WorkerProfile)
			.FirstOrDefaultAsync(u => u.Id == userId);

		if (user is null)
		{
			FailWithReason(context, "Account not found", "No user found for the provided credentials.");
			return;
		}

		IEnumerable<string> userRoles = user.GetUserRoles().Select(r => r.ToString());
		IReadOnlyList<string>? tokenRoles = context.Principal?.GetRoles();

		if (!ValidateRoles(userRoles, tokenRoles, out string? error))
		{
			FailWithReason(context, "Role mismatch", error, StatusCodes.Status403Forbidden);
			return;
		}

		Guid? dbSecurityStamp = user.SecurityStamp;
		bool isAccountLocked = user.IsLocked;

		if (isAccountLocked)
		{
			FailWithReason(context, "Account locked", "Your account is locked. Please contact support for help unlocking it.");
			return;
		}

		if (dbSecurityStamp is null)
		{
			FailWithReason(context, "Account removed", "This account no longer exists or has been removed.");
			return;
		}

		if (dbSecurityStamp.Value == Guid.Empty)
		{
			FailWithReason(context, "Signed out", "You've been signed out. Please log in again to continue.");
			return;
		}

		if (!string.Equals(dbSecurityStamp.Value.ToString(), securityStampClaim, StringComparison.Ordinal))
		{
			FailWithReason(context, "Session expired", "Your login session is no longer valid. Please sign in again.");
			return;
		}
	}

	private static Task HandleChallengeAsync(JwtBearerChallengeContext context, JsonSerializerOptions jsonOptions)
	{
		context.HandleResponse();

		string? userMessage = context.HttpContext.Items[AuthMessageKey] as string;
		string? errorReason = context.HttpContext.Items[AuthErrorKey] as string;
		int statusCode = context.HttpContext.Items[AuthStatusCodeKey] is int code && code >= 400 && code < 600
			? code
			: StatusCodes.Status401Unauthorized;

		return WriteJsonErrorAsync(
			context.Response,
			statusCode,
			userMessage ?? "You need to be signed in to access this feature.",
			errorReason is not null ? [errorReason] : null,
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
		userId = principal?.GetUserId().ToString();
		securityStamp = principal?.GetSecurityStamp().ToString();
		tokenType = principal?.GetTokenType();
	}

	private static void FailWithReason(TokenValidatedContext context, string? message = null, string? reason = null, int statusCode = StatusCodes.Status401Unauthorized)
	{
		context.HttpContext.Items[AuthMessageKey] = string.IsNullOrWhiteSpace(message)
			? "Your session is no longer valid. Please sign in again."
			: message;

		string failReason = string.IsNullOrWhiteSpace(reason)
			? "Token validation failed."
			: reason;

		context.HttpContext.Items[AuthErrorKey] = failReason;
		context.HttpContext.Items[AuthStatusCodeKey] = statusCode;
		context.Fail(failReason);
	}

	private static Task WriteJsonErrorAsync(HttpResponse response, int statusCode, string? message, List<string>? errors, JsonSerializerOptions options)
	{
		response.StatusCode = statusCode;
		response.ContentType = "application/json";

		var payload = ApiResponse<object>.Fail(null, message, errors);
		string json = JsonSerializer.Serialize(payload, options);

		return response.WriteAsync(json);
	}

	private static bool ValidateRoles(IEnumerable<string> dbRoles, IEnumerable<string>? tokenRoles, out string? errorMessage)
	{
		errorMessage = null;

		if (tokenRoles is null)
		{
			errorMessage = "Unable to verify your account roles. Please sign in again.";
			return false;
		}

		List<string> dbRolesList = [.. dbRoles];
		List<string> tokenRolesList = [.. tokenRoles];

		// Must match count + content (strict equality, case-insensitive)
		if (dbRolesList.Count != tokenRolesList.Count ||
			!dbRolesList.All(r => tokenRolesList.Contains(r, StringComparer.OrdinalIgnoreCase)))
		{
			errorMessage = "Your account roles have changed. Please refresh your session.";
			return false;
		}

		return true;
	}

	#endregion
}
