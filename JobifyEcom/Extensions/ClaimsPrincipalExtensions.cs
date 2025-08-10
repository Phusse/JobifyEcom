using System.Security.Claims;

namespace JobifyEcom.Extensions;

/// <summary>
/// Provides extension methods for extracting user-related information from a <see cref="ClaimsPrincipal"/>.
/// </summary>
public static class ClaimsPrincipalExtensions
{
	/// <summary>
	/// Attempts to extract the user's ID from their claims.
	/// </summary>
	/// <param name="user">The current user's claims principal.</param>
	/// <param name="userId">The parsed <see cref="Guid"/> of the user if successful.</param>
	/// <returns><c>true</c> if the user ID was successfully extracted; otherwise, <c>false</c>.</returns>
	public static bool TryGetUserId(this ClaimsPrincipal user, out Guid userId)
	{
		Claim? claim = user.FindFirst(ClaimTypes.NameIdentifier);
		return Guid.TryParse(claim?.Value, out userId);
	}
}