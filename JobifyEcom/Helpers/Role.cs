using JobifyEcom.Enums;

namespace JobifyEcom.Helpers;

/// <summary>
/// Contains string constants representing user roles for authorization purposes.
/// These constants map directly to the names of the <see cref="UserRole"/> enum values
/// and are used to avoid hardcoding role names throughout the application.
/// </summary>
public static class Roles
{
	/// <summary>
	/// The role assigned to users who perform services or fulfill jobs.
	/// </summary>
	public const string Worker = nameof(UserRole.Worker);

	/// <summary>
	/// The role assigned to users who have full administrative access.
	/// </summary>
	public const string Admin = nameof(UserRole.Admin);

	/// <summary>
	/// The role assigned to regular users who request or purchase services.
	/// </summary>
	public const string Customer = nameof(UserRole.Customer);
}
