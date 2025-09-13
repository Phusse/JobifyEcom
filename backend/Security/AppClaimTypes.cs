using System.Security.Claims;

namespace JobifyEcom.Security;

/// <summary>
/// Defines custom claim types used in the application.
/// </summary>
internal static class AppClaimTypes
{
	/// <summary>
	/// Claim type for the user's unique identifier.
	/// </summary>
	internal static readonly string UserId = ClaimTypes.NameIdentifier;

	/// <summary>
	/// Claim type for the user's email address.
	/// </summary>
	internal static readonly string Email = ClaimTypes.Email;

	/// <summary>
	/// Claim type for the user's role.
	/// </summary>
	internal static readonly string Role = ClaimTypes.Role;

	/// <summary>
	/// Claim type for the security stamp for token invalidation.
	/// </summary>
	internal const string SecurityStamp = "security_stamp";

	/// <summary>
	/// Claim type for the token type (Access or Refresh).
	/// </summary>
	internal const string TokenType = "token_type";
}
