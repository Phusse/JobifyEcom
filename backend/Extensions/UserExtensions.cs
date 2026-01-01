using JobifyEcom.Enums;
using JobifyEcom.Models;

namespace JobifyEcom.Extensions;

/// <summary>
/// Internal extension methods for <see cref="User"/> objects.
/// </summary>
internal static class UserExtensions
{
    extension(User user)
    {
        /// <summary>
        /// Gets the effective system roles for the <see cref="User"/>.
        /// Always includes <see cref="SystemRole.User"/>.
        /// Adds <see cref="SystemRole.Worker"/> if <see cref="User.IsWorker"/> is true,
        /// and the staff role if <see cref="User.StaffRole"/> is set.
        /// <para>
        /// <b>Important:</b> <see cref="User.WorkerProfile"/> is a navigation property,
        /// make sure it is eagerly loaded (e.g. with <c>.Include(u =&gt; u.Worker)</c>)
        /// before calling this method. Otherwise <see cref="User.IsWorker"/> may return
        /// <c>false</c> even for users that are workers.
        /// </para>
        /// </summary>
        /// <returns>A list of assigned <see cref="SystemRole"/> values.</returns>
        internal List<SystemRole> GetUserRoles()
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
}
