using JobifyEcom.Exceptions;

namespace JobifyEcom.Contracts.Errors;

internal static partial class ErrorCatalog
{
	internal static readonly ErrorDefinition AccountNotFound = new(
		Id: "USER_NOT_FOUND",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "We couldn't find your account. Please contact support if this issue continues.",
		Details: []
	);

	internal static readonly ErrorDefinition InvalidStaffRole = new(
		Id: "USER_INVALID_STAFF_ROLE",
		HttpStatus: StatusCodes.Status400BadRequest,
		Title: "Staff Role is invalid.",
		Details: ["Staff Role can only be Admin, SuperAdmin, or null."]
	);

	internal static readonly ErrorDefinition WorkerProfileMissing = new(
		Id: "WORKER_PROFILE_MISSING",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "Your worker profile is missing. Please complete your profile before applying.",
		Details: []
	);

	internal static readonly ErrorDefinition WorkerProfileExists = new(
		Id: "WORKER_PROFILE_EXISTS",
		HttpStatus: StatusCodes.Status409Conflict,
		Title: "A worker profile already exists for this account.",
		Details: []
	);

	internal static readonly ErrorDefinition WorkerProfileNotFound = new(
		Id: "WORKER_PROFILE_NOT_FOUND",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "No worker profile could be found for this user.",
		Details: []
	);
}
