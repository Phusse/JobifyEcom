using JobifyEcom.Contracts.Responses;

namespace JobifyEcom.Contracts.Errors;

internal static partial class ErrorCatalog
{
	internal static readonly ErrorResponseDefinition Unauthorized = new(
		Id: "AUTH_UNAUTHORIZED",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "You need to be signed in to access this resource.",
		Details: []
	);

	internal static readonly ErrorResponseDefinition Forbidden = new(
		Id: "AUTH_FORBIDDEN",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "You do not have permission to perform this action.",
		Details: []
	);

	internal static readonly ErrorResponseDefinition InvalidTokenType = new(
		Id: "AUTH_INVALID_TOKEN",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "Invalid session.",
		Details: ["Token type was not 'Access' during JWT validation."]
	);

	internal static readonly ErrorResponseDefinition SessionNotVerified = new(
		Id: "AUTH_SESSION_NOT_VERIFIED",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "Your login session could not be verified. Please sign in again.",
		Details: []
	);

	internal static readonly ErrorResponseDefinition AccountNotConfirmed = new(
		Id: "AUTH_ACCOUNT_NOT_CONFIRMED",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "We couldn't confirm your account details. Please sign in again.",
		Details: []
	);

	internal static readonly ErrorResponseDefinition AccountNotFoundJwt = new(
		Id: "AUTH_ACCOUNT_NOT_FOUND",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "No user found for the provided credentials.",
		Details: []
	);

	internal static readonly ErrorResponseDefinition RoleMismatch = new(
		Id: "AUTH_ROLE_MISMATCH",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "Your account roles have changed. Please refresh your session.",
		Details: []
	);

	internal static readonly ErrorResponseDefinition AccountLocked = new(
		Id: "AUTH_ACCOUNT_LOCKED",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "Your account is locked. Please contact support for help unlocking it.",
		Details: []
	);

	internal static readonly ErrorResponseDefinition AccountRemoved = new(
		Id: "AUTH_ACCOUNT_REMOVED",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "This account no longer exists or has been removed.",
		Details: []
	);

	internal static readonly ErrorResponseDefinition SignedOut = new(
		Id: "AUTH_SIGNED_OUT",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "You've been signed out. Please log in again to continue.",
		Details: []
	);

	internal static readonly ErrorResponseDefinition SessionExpired = new(
		Id: "AUTH_SESSION_EXPIRED",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "Your login session is no longer valid. Please sign in again.",
		Details: []
	);

	internal static readonly ErrorResponseDefinition MissingLoginDetails = new(
		Id: "AUTH_MISSING_LOGIN_DETAILS",
		HttpStatus: StatusCodes.Status400BadRequest,
		Title: "Missing login details.",
		Details: ["Please enter both your email address and password to sign in."]
	);

	internal static readonly ErrorResponseDefinition InvalidCredentials = new(
		Id: "AUTH_INVALID_CREDENTIALS",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "Login failed.",
		Details: ["We couldn't find an account with those credentials. Please check your email and password, then try again."]
	);

	internal static readonly ErrorResponseDefinition EmailNotConfirmed = new(
		Id: "AUTH_EMAIL_NOT_CONFIRMED",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "Email confirmation required.",
		Details: ["Please confirm your email address before signing in. Check your inbox for the confirmation link."]
	);

	internal static readonly ErrorResponseDefinition MissingRefreshToken = new(
		Id: "AUTH_MISSING_REFRESH_TOKEN",
		HttpStatus: StatusCodes.Status400BadRequest,
		Title: "No refresh token provided.",
		Details: ["A refresh token is required to renew your session. Please provide a valid refresh token."]
	);

	internal static readonly ErrorResponseDefinition SessionInvalid = new(
		Id: "AUTH_SESSION_INVALID",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "Session invalid.",
		Details: ["Your account security has changed. Please sign in again to continue."]
	);

	internal static readonly ErrorResponseDefinition TokenTypeInvalid = new(
		Id: "AUTH_INVALID_TOKEN_TYPE",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "Invalid token type.",
		Details: ["The provided token does not match the expected type."]
	);

	internal static readonly ErrorResponseDefinition SessionDataInvalid = new(
		Id: "AUTH_SESSION_DATA_INVALID",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "Session data invalid.",
		Details: ["We couldn't verify your session details. Please login again."]
	);

	internal static readonly ErrorResponseDefinition AlreadyRegistered = new(
		Id: "AUTH_EMAIL_ALREADY_REGISTERED",
		HttpStatus: StatusCodes.Status409Conflict,
		Title: "Email already registered.",
		Details: ["An account with this email address already exists. Please sign in or use a different email."]
	);

	internal static readonly ErrorResponseDefinition AuthenticationRequired = new(
		Id: "AUTH_REQUIRED",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "Authentication required.",
		Details: ["You must be signed in to log out."]
	);
}
