using JobifyEcom.Exceptions;

namespace JobifyEcom.Contracts.Errors;

internal static partial class ErrorCatalog
{
	internal static readonly ErrorDefinition ValidationFailed = new(
		Code: "VALIDATION_FAILED",
		HttpStatus: StatusCodes.Status400BadRequest,
		Title: "One or more validation errors occurred.",
		Details: []
	);

	internal static readonly ErrorDefinition Conflict = new(
		Code: "RESOURCE_CONFLICT",
		HttpStatus: StatusCodes.Status409Conflict,
		Title: "The request conflicts with the current state of the resource.",
		Details: []
	);

	internal static readonly ErrorDefinition InternalServerError = new(
		Code: "INTERNAL_SERVER_ERROR",
		HttpStatus: StatusCodes.Status500InternalServerError,
		Title: "An unexpected error occurred. Please try again later.",
		Details: []
	);
}
