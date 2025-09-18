using System.Net.Mail;

namespace JobifyEcom.Helpers;

/// <summary>
/// Provides utility methods for common validation tasks.
/// </summary>
internal static class ValidationHelper
{
	/// <summary>
	/// Determines whether the specified string is a valid email address.
	/// </summary>
	/// <param name="email">The email address string to validate.</param>
	/// <returns>
	/// <c>true</c> if the input is a valid email address; otherwise, <c>false</c>.
	/// </returns>
	/// <remarks>
	/// This method uses <see cref="MailAddress"/> from <c>System.Net.Mail</c> to parse the email.
	/// If parsing fails, the email is considered invalid. The comparison ensures that the normalized
	/// address matches the original input string.
	/// </remarks>
	internal static bool IsValidEmail(string email)
	{
		try
		{
			var addr = new MailAddress(email);
			return addr.Address == email;
		}
		catch
		{
			return false;
		}
	}
}
