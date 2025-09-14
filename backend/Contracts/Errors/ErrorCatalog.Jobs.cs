using JobifyEcom.Exceptions;

namespace JobifyEcom.Contracts.Errors;

internal static partial class ErrorCatalog
{
	internal static readonly ErrorDefinition JobNotFound = new(
		Id: "JOB_NOT_FOUND",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "The requested job could not be found.",
		Details: []
	);

	internal static readonly ErrorDefinition AlreadyApplied = new(
		Id: "JOB_ALREADY_APPLIED",
		HttpStatus: StatusCodes.Status409Conflict,
		Title: "You have already applied for this job.",
		Details: []
	);

	internal static readonly ErrorDefinition CannotApplyToOwnJob = new(
		Id: "JOB_APPLY_OWN",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "You cannot apply to a job you posted.",
		Details: []
	);

	internal static readonly ErrorDefinition ApplicationNotFound = new(
		Id: "APPLICATION_NOT_FOUND",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "No application was found for the specified job.",
		Details: []
	);

	internal static readonly ErrorDefinition ApplicationForbidden = new(
		Id: "APPLICATION_FORBIDDEN",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "Only the applicant or the job poster can view this application.",
		Details: []
	);

	internal static readonly ErrorDefinition ApplicationStatusForbidden = new(
		Id: "APPLICATION_STATUS_FORBIDDEN",
		HttpStatus: StatusCodes.Status403Forbidden,
		Title: "Only the job poster can update the status of applications.",
		Details: []
	);
}
