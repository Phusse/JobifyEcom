using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace JobifyEcom.Helpers;

/// <summary>
/// Provides utility methods for working with enums that use <see cref="DisplayAttribute"/>.
/// </summary>
/// <remarks>
/// This helper is designed to simplify retrieving user-friendly names for enum values.
/// By default, <see cref="Enum.ToString()"/> returns the enum field name, which is often
/// not suitable for display in UI or API responses.
///
/// Applying a <see cref="DisplayAttribute"/> to enum members allows you to define a
/// localized, human-readable name. This helper checks for the attribute and falls back
/// to the enum's identifier if the attribute is not present.
///
/// Example:
/// <code>
/// public enum UserRole
/// {
///     [Display(Name = "Administrator")]
///     Admin,
///
///     [Display(Name = "Regular User")]
///     User
/// }
///
/// var name = EnumDisplayHelper.GetDisplayName(UserRole.Admin); // "Administrator"
/// </code>
/// </remarks>
public static class EnumDisplayHelper
{
	/// <summary>
	/// Gets the display name for a given enum value.
	/// </summary>
	/// <typeparam name="T">The enum type.</typeparam>
	/// <param name="value">The enum value.</param>
	/// <returns>
	/// The <see cref="DisplayAttribute.Name"/> if present;
	/// otherwise, the enum value's name as a string.
	/// </returns>
	public static string GetDisplayName<T>(T value) where T : Enum
	{
		MemberInfo? member = typeof(T).GetMember(value.ToString()).FirstOrDefault();
		var displayAttr = member?.GetCustomAttribute<DisplayAttribute>();
		return displayAttr?.Name ?? value.ToString();
	}
}
