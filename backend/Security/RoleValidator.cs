namespace JobifyEcom.Security;

/// <summary>
/// Provides logic to verify that a user's roles stored in the database
/// are consistent with the roles embedded in their JWT token.
/// </summary>
internal static class RoleValidator
{
	/// <summary>
	/// Validates that the roles from the database and the roles from the token
	/// are an exact match, using a case-insensitive comparison.
	/// </summary>
	internal static bool Validate(IEnumerable<string> dbRoles, IEnumerable<string> tokenRoles)
	{
		// Use HashSets for efficient comparison and to remove duplicates
		HashSet<string> dbRoleSet = new(dbRoles, StringComparer.OrdinalIgnoreCase);
		HashSet<string> tokenRoleSet = new(tokenRoles, StringComparer.OrdinalIgnoreCase);

		if (!dbRoleSet.SetEquals(tokenRoleSet)) return false;

		return true;
	}
}
