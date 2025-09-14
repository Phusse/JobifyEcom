using JobifyEcom.Exceptions;

namespace JobifyEcom.Contracts.Errors;

internal static partial class ErrorCatalog
{
	internal static readonly ErrorDefinition Unauthorized = new(
		Id: "AUTH_UNAUTHORIZED",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "You need to be signed in to access this resource.",
		Details: []
	);

	internal static readonly ErrorDefinition Forbidden = new(
		Id: "AUTH_FORBIDDEN",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "You do not have permission to perform this action.",
		Details: []
	);

	internal static readonly ErrorDefinition InvalidTokenType = new(
		Id: "AUTH_INVALID_TOKEN",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "Invalid session.",
		Details: ["Token type was not 'Access' during JWT validation."]
	);

	internal static readonly ErrorDefinition SessionNotVerified = new(
		Id: "AUTH_SESSION_NOT_VERIFIED",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "Your login session could not be verified. Please sign in again.",
		Details: []
	);

	internal static readonly ErrorDefinition AccountNotConfirmed = new(
		Id: "AUTH_ACCOUNT_NOT_CONFIRMED",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "We couldn't confirm your account details. Please sign in again.",
		Details: []
	);

	internal static readonly ErrorDefinition AccountNotFoundJwt = new(
		Id: "AUTH_ACCOUNT_NOT_FOUND",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "No user found for the provided credentials.",
		Details: []
	);

	internal static readonly ErrorDefinition RoleMismatch = new(
		Id: "AUTH_ROLE_MISMATCH",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "Your account roles have changed. Please refresh your session.",
		Details: []
	);

	internal static readonly ErrorDefinition AccountLocked = new(
		Id: "AUTH_ACCOUNT_LOCKED",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "Your account is locked. Please contact support for help unlocking it.",
		Details: []
	);

	internal static readonly ErrorDefinition AccountRemoved = new(
		Id: "AUTH_ACCOUNT_REMOVED",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "This account no longer exists or has been removed.",
		Details: []
	);

	internal static readonly ErrorDefinition SignedOut = new(
		Id: "AUTH_SIGNED_OUT",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "You've been signed out. Please log in again to continue.",
		Details: []
	);

	internal static readonly ErrorDefinition SessionExpired = new(
		Id: "AUTH_SESSION_EXPIRED",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "Your login session is no longer valid. Please sign in again.",
		Details: []
	);
}
