using JobifyEcom.Exceptions;

namespace JobifyEcom.Contracts;

/// <summary>
/// Central registry of all application error definitions.
/// Each error includes a unique code, an HTTP status, and user-facing text.
/// </summary>
public static class ErrorCatalog
{
	// =========================
	// Authentication & Security
	// =========================
	public static readonly ErrorDefinition Unauthorized = new(
		Code: "AUTH_UNAUTHORIZED",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "You need to be signed in to access this resource.",
		Details: []
	);

	public static readonly ErrorDefinition Forbidden = new(
		Code: "AUTH_FORBIDDEN",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "You do not have permission to perform this action.",
		Details: []
	);

	public static readonly ErrorDefinition InvalidSession = new(
		Code: "AUTH_INVALID_SESSION",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "Invalid session.",
		Details: ["Token type was not 'Access' during JWT validation."]
	);

	public static readonly ErrorDefinition SessionNotVerified = new(
		Code: "AUTH_SESSION_NOT_VERIFIED",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "Your login session could not be verified. Please sign in again.",
		Details: []
	);

	public static readonly ErrorDefinition AccountNotConfirmed = new(
		Code: "AUTH_ACCOUNT_NOT_CONFIRMED",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "We couldn't confirm your account details. Please sign in again.",
		Details: []
	);

	public static readonly ErrorDefinition AccountNotFoundJwt = new(
		Code: "AUTH_ACCOUNT_NOT_FOUND",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "No user found for the provided credentials.",
		Details: []
	);

	public static readonly ErrorDefinition RoleMismatch = new(
		Code: "AUTH_ROLE_MISMATCH",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "Your account roles have changed. Please refresh your session.",
		Details: []
	);

	public static readonly ErrorDefinition AccountLocked = new(
		Code: "AUTH_ACCOUNT_LOCKED",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "Your account is locked. Please contact support for help unlocking it.",
		Details: []
	);

	public static readonly ErrorDefinition AccountRemoved = new(
		Code: "AUTH_ACCOUNT_REMOVED",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "This account no longer exists or has been removed.",
		Details: []
	);

	public static readonly ErrorDefinition SignedOut = new(
		Code: "AUTH_SIGNED_OUT",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "You've been signed out. Please log in again to continue.",
		Details: []
	);

	public static readonly ErrorDefinition SessionExpired = new(
		Code: "AUTH_SESSION_EXPIRED",
		HttpStatus: StatusCodes.Status401Unauthorized,
		Title: "Your login session is no longer valid. Please sign in again.",
		Details: []
	);

	// =================
	// Users & Profiles
	// =================
	public static readonly ErrorDefinition AccountNotFound = new(
		Code: "USER_NOT_FOUND",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "We couldn't find your account. Please contact support if this issue continues.",
		Details: []
	);

	public static readonly ErrorDefinition WorkerProfileMissing = new(
		Code: "WORKER_PROFILE_MISSING",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "Your worker profile is missing. Please complete your profile before applying.",
		Details: []
	);

	public static readonly ErrorDefinition WorkerProfileExists = new(
		Code: "WORKER_PROFILE_EXISTS",
		HttpStatus: StatusCodes.Status409Conflict,
		Title: "A worker profile already exists for this account.",
		Details: []
	);

	public static readonly ErrorDefinition WorkerProfileNotFound = new(
		Code: "WORKER_PROFILE_NOT_FOUND",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "No worker profile could be found for this user.",
		Details: []
	);

	// ================
	// Jobs & Applications
	// ================
	public static readonly ErrorDefinition JobNotFound = new(
		Code: "JOB_NOT_FOUND",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "The requested job could not be found.",
		Details: []
	);

	public static readonly ErrorDefinition AlreadyApplied = new(
		Code: "JOB_ALREADY_APPLIED",
		HttpStatus: StatusCodes.Status409Conflict,
		Title: "You have already applied for this job.",
		Details: []
	);

	public static readonly ErrorDefinition CannotApplyToOwnJob = new(
		Code: "JOB_APPLY_OWN",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "You cannot apply to a job you posted.",
		Details: []
	);

	public static readonly ErrorDefinition ApplicationNotFound = new(
		Code: "APPLICATION_NOT_FOUND",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "No application was found for the specified job.",
		Details: []
	);

	public static readonly ErrorDefinition ApplicationForbidden = new(
		Code: "APPLICATION_FORBIDDEN",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "Only the applicant or the job poster can view this application.",
		Details: []
	);

	public static readonly ErrorDefinition ApplicationStatusForbidden = new(
		Code: "APPLICATION_STATUS_FORBIDDEN",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "Only the job poster can update the status of applications.",
		Details: []
	);

	// =================
	// Skills & Verification
	// =================
	public static readonly ErrorDefinition SkillNotFound = new(
		Code: "SKILL_NOT_FOUND",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "The requested skill could not be found.",
		Details: []
	);

	public static readonly ErrorDefinition VerificationNotFound = new(
		Code: "VERIFICATION_NOT_FOUND",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "Verification record not found for this skill.",
		Details: []
	);

	public static readonly ErrorDefinition TagRequired = new(
		Code: "SKILL_TAG_REQUIRED",
		HttpStatus: StatusCodes.Status400BadRequest,
		Title: "At least one tag is required for a skill.",
		Details: []
	);

	// ============
	// Generic
	// ============
	public static readonly ErrorDefinition ValidationFailed = new(
		Code: "VALIDATION_FAILED",
		HttpStatus: StatusCodes.Status400BadRequest,
		Title: "One or more validation errors occurred.",
		Details: []
	);

	public static readonly ErrorDefinition Conflict = new(
		Code: "RESOURCE_CONFLICT",
		HttpStatus: StatusCodes.Status409Conflict,
		Title: "The request conflicts with the current state of the resource.",
		Details: []
	);

	public static readonly ErrorDefinition InternalServerError = new(
		Code: "INTERNAL_SERVER_ERROR",
		HttpStatus: StatusCodes.Status500InternalServerError,
		Title: "An unexpected error occurred. Please try again later.",
		Details: []
	);
}
