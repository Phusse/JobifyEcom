using JobifyEcom.Enums;
using JobifyEcom.Models;

namespace JobifyEcom.Extensions;

/// <summary>
/// Provides extension methods for working with <see cref="User"/> objects.
/// </summary>
public static class UserExtensions
{
	/// <summary>
	/// Retrieves the effective system roles assigned to the specified <see cref="User"/>.
	/// </summary>
	/// <param name="user">
	/// The <see cref="User"/> instance from which to determine the roles.
	/// </param>
	/// <returns>
	/// A <see cref="List{SystemRole}"/> containing all roles assigned to the user.
	/// Always includes <see cref="SystemRole.User"/> by default.
	/// </returns>
	/// <remarks>
	/// <list type="bullet">
	/// <item>
	/// <see cref="SystemRole.User"/> is always included.
	/// </item>
	/// <item>
	/// If <see cref="User.IsWorker"/> is <c>true</c>, the <see cref="SystemRole.Worker"/> role is included.
	/// </item>
	/// <item>
	/// If <see cref="User.StaffRole"/> is not <c>null</c>, the staff role is included.
	/// </item>
	/// </list>
	/// </remarks>
	public static List<SystemRole> GetUserRoles(this User user)
	{
		List<SystemRole> userRoles = [SystemRole.User];

		if (user.IsWorker)
		{
			userRoles.Add(SystemRole.Worker);
		}

		if (user.StaffRole is not null)
		{
			userRoles.Add(user.StaffRole.Value);
		}

		return userRoles;
	}
}
