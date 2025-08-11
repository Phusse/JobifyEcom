namespace JobifyEcom.Security;

/// <summary>
/// Specifies the type of JSON Web Token (JWT) issued to a user.
/// </summary>
/// <remarks>
/// This enum is used to differentiate between short-lived <see cref="Access"/> tokens
/// and longer-lived <see cref="Refresh"/> tokens in authentication and authorization flows.
/// </remarks>
public enum TokenType
{
	/// <summary>
	/// Grants temporary access to protected resources.
	/// Typically expires within minutes or hours and must be included
	/// in requests to secured endpoints.
	/// </summary>
	Access,

	/// <summary>
	/// Used to obtain a new <see cref="Access"/> token without requiring the user to log in again.
	/// Usually has a longer lifetime than an access token.
	/// </summary>
	Refresh,
}
