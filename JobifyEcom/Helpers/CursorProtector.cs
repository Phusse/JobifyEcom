using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using JobifyEcom.Models;

namespace JobifyEcom.Helpers;

/// <summary>
/// Provides functionality to protect and validate cursor-based pagination state.
/// </summary>
/// <remarks>
/// Encodes <see cref="CursorState"/> objects as signed Base64 strings using
/// HMAC-SHA256 with a secret key loaded from configuration (<c>CursorOptions:SecretKey</c>).
/// This ensures that the cursor cannot be tampered with between requests,
/// while remaining compact and URL-safe.
/// </remarks>
public class CursorProtector
{
	private readonly byte[] _key;

	/// <summary>
	/// Initializes a new instance of the <see cref="CursorProtector"/> class.
	/// </summary>
	/// <param name="config">
	/// The application configuration. Must contain a non-empty
	/// <c>CursorOptions:SecretKey</c> entry.
	/// </param>
	/// <exception cref="InvalidOperationException">
	/// Thrown if <c>CursorOptions:SecretKey</c> is not configured.
	/// </exception>
	public CursorProtector(IConfiguration config)
	{
		string? secretKey = config["CursorOptions:SecretKey"];

		if (string.IsNullOrWhiteSpace(secretKey))
			throw new InvalidOperationException("CursorOptions:SecretKey is not configured.");

		_key = Encoding.UTF8.GetBytes(secretKey);
	}

	/// <summary>
	/// Encodes a <see cref="CursorState"/> into a signed Base64 string.
	/// </summary>
	/// <param name="state">The cursor state to encode.</param>
	/// <returns>
	/// A Base64 string containing the serialized cursor state with an HMAC signature appended.
	/// </returns>
	public string Encode(CursorState state)
	{
		string json = JsonSerializer.Serialize(state);
		byte[] data = Encoding.UTF8.GetBytes(json);

		using var hmac = new HMACSHA256(_key);
		byte[] signature = hmac.ComputeHash(data);

		byte[] payload = data.Concat(signature).ToArray();
		return Convert.ToBase64String(payload);
	}

	/// <summary>
	/// Decodes and validates a previously encoded cursor token.
	/// </summary>
	/// <param name="token">The Base64-encoded cursor token.</param>
	/// <returns>The deserialized <see cref="CursorState"/> object.</returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown if the token has been tampered with or the signature is invalid.
	/// </exception>
	public CursorState Decode(string token)
	{
		byte[] payload = Convert.FromBase64String(token);

		int sigLength = 32; // HMACSHA256 = 32 bytes
		byte[] data = payload.Take(payload.Length - sigLength).ToArray();
		byte[] signature = payload.Skip(payload.Length - sigLength).ToArray();

		using var hmac = new HMACSHA256(_key);
		byte[] expectedSig = hmac.ComputeHash(data);

		if (!expectedSig.SequenceEqual(signature))
			throw new InvalidOperationException("Invalid cursor token.");

		string json = Encoding.UTF8.GetString(data);
		return JsonSerializer.Deserialize<CursorState>(json)!;
	}
}
