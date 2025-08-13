using System.Security.Claims;

namespace JobifyEcom.Extensions;

/// <summary>
/// Extension methods for extracting strongly-typed values from a <see cref="ClaimsPrincipal"/>.
/// </summary>
public static class ClaimsPrincipalExtensions
{
	/// <summary>
	/// Gets the authenticated user's ID from claims.
	/// </summary>
	public static Guid? GetUserId(this ClaimsPrincipal user)
	{
		string? value = user.FindFirstValue(ClaimTypes.NameIdentifier);
		return Guid.TryParse(value, out var id) ? id : null;
	}

	/// <summary>
	/// Gets the authenticated user's email from claims.
	/// </summary>
	public static string? GetEmail(this ClaimsPrincipal user)
	{
		return user.FindFirstValue("email");
	}

	/// <summary>
	/// Gets the authenticated user's role from claims.
	/// </summary>
	public static string? GetRole(this ClaimsPrincipal user)
	{
		return user.FindFirstValue("role");
	}

	/// <summary>
	/// Gets the security stamp for the user's current session from claims.
	/// </summary>
	public static Guid? GetSecurityStamp(this ClaimsPrincipal user)
	{
		string? value = user.FindFirstValue("security_stamp");
		return Guid.TryParse(value, out var stamp) ? stamp : null;
	}

	/// <summary>
	/// Gets the token type (Access or Refresh) from claims.
	/// </summary>
	public static string? GetTokenType(this ClaimsPrincipal user)
	{
		return user.FindFirstValue("token_type");
	}
}
