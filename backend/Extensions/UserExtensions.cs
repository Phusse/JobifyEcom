using JobifyEcom.Enums;
using JobifyEcom.Models;

namespace JobifyEcom.Extensions;

/// <summary>
/// Internal extension methods for <see cref="User"/> objects.
/// </summary>
internal static class UserExtensions
{
	/// <summary>
	/// Gets the effective system roles for the <see cref="User"/>.
	/// Always includes <see cref="SystemRole.User"/>. Adds <see cref="SystemRole.Worker"/> if <see cref="User.IsWorker"/>
	/// and the staff role if <see cref="User.StaffRole"/> is set.
	/// </summary>
	/// <param name="user">The <see cref="User"/> instance.</param>
	/// <returns>List of assigned <see cref="SystemRole"/>.</returns>
	internal static List<SystemRole> GetUserRoles(this User user)
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
