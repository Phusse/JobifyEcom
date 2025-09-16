using JobifyEcom.Contracts.Responses;

namespace JobifyEcom.Contracts.Errors;

internal static partial class ErrorCatalog
{
	internal static readonly ErrorResponseDefinition AccountNotFound = new(
		Id: "USER_NOT_FOUND",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "We couldn't find your account. Please contact support if this issue continues.",
		Details: []
	);

	internal static readonly ErrorResponseDefinition InvalidStaffRole = new(
		Id: "USER_INVALID_STAFF_ROLE",
		HttpStatus: StatusCodes.Status400BadRequest,
		Title: "Staff Role is invalid.",
		Details: ["Staff Role can only be Admin, SuperAdmin, or null."]
	);

	internal static readonly ErrorResponseDefinition PasswordResetUnavailableForLockedAccount = new(
		Id: "USER_PASSWORD_RESET_LOCKED",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "Password reset unavailable.",
		Details: ["This account is locked and cannot reset the password."]
	);

	internal static readonly ErrorResponseDefinition PasswordResetLinkExpired = new(
		Id: "USER_PASSWORD_RESET_LINK_EXPIRED",
		HttpStatus: StatusCodes.Status410Gone,
		Title: "Password reset failed.",
		Details: ["The reset link has expired. Please request a new one."]
	);

	internal static readonly ErrorResponseDefinition PasswordResetMissingNewPassword = new(
		Id: "USER_PASSWORD_RESET_MISSING_NEW_PASSWORD",
		HttpStatus: StatusCodes.Status400BadRequest,
		Title: "Password reset failed.",
		Details: ["Please enter a new password."]
	);


	internal static readonly ErrorResponseDefinition WorkerProfileMissing = new(
		Id: "WORKER_PROFILE_MISSING",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "Your worker profile is missing. Please complete your profile before applying.",
		Details: []
	);

	internal static readonly ErrorResponseDefinition WorkerProfileExists = new(
		Id: "WORKER_PROFILE_EXISTS",
		HttpStatus: StatusCodes.Status409Conflict,
		Title: "A worker profile already exists for this account.",
		Details: []
	);

	internal static readonly ErrorResponseDefinition WorkerProfileNotFound = new(
		Id: "WORKER_PROFILE_NOT_FOUND",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "No worker profile could be found for this user.",
		Details: []
	);

	internal static readonly ErrorResponseDefinition AccountNotFoundByEmail = new(
		Id: "USER_NOT_FOUND_BY_EMAIL",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "Unable to confirm email.",
		Details: ["No account found with this email address. Please check and try again."]
	);

	internal static readonly ErrorResponseDefinition EmailConfirmationTokenInvalid = new(
		Id: "USER_EMAIL_CONFIRMATION_INVALID",
		HttpStatus: StatusCodes.Status400BadRequest,
		Title: "Unable to confirm email.",
		Details: ["This confirmation link is no longer valid. Please request a new one."]
	);

	internal static readonly ErrorResponseDefinition SelfModificationNotAllowed = new(
		Id: "USER_SELF_MODIFICATION_FORBIDDEN",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "Operation not permitted.",
		Details: ["You cannot perform this action on your own account using this endpoint."]
	);

	internal static readonly ErrorResponseDefinition AdminCannotModifySuperAdmin = new(
		Id: "USER_ADMIN_MODIFY_SUPERADMIN_FORBIDDEN",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "Operation not permitted.",
		Details: ["Admins cannot modify SuperAdmin accounts."]
	);

	internal static readonly ErrorResponseDefinition AdminCannotModifyAdmin = new(
		Id: "USER_ADMIN_MODIFY_ADMIN_FORBIDDEN",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "Operation not permitted.",
		Details: ["Admins cannot modify other Admin accounts."]
	);

	internal static readonly ErrorResponseDefinition SuperAdminRequiredForModification = new(
		Id: "USER_SUPERADMIN_REQUIRED_FOR_MODIFICATION",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "Operation not permitted.",
		Details: ["Only SuperAdmins can modify SuperAdmin accounts."]
	);

}
