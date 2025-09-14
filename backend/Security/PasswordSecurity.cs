using System.Security.Cryptography;
using System.Text;

namespace JobifyEcom.Security;

/// <summary>
/// Provides methods for hashing and verifying passwords using SHA-256.
/// </summary>
internal static class PasswordSecurity
{
	/// <summary>
	/// Hashes a password using SHA-256 and encodes it as Base64.
	/// </summary>
	/// <param name="password">The plain text password to hash.</param>
	/// <returns>The hashed password as a Base64 string.</returns>
	internal static string HashPassword(string password)
	{
		byte[] hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
		return Convert.ToBase64String(hashedBytes);
	}

	/// <summary>
	/// Verifies that a plain text password matches a previously hashed password.
	/// </summary>
	/// <param name="inputPassword">The plain text password to verify.</param>
	/// <param name="storedHash">The previously hashed password to compare against.</param>
	/// <returns><c>true</c> if the password matches; otherwise, <c>false</c>.</returns>
	internal static bool VerifyPassword(string inputPassword, string storedHash)
	{
		return HashPassword(inputPassword) == storedHash;
	}
}
