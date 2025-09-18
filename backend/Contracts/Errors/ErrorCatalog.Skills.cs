using JobifyEcom.Contracts.Responses;

namespace JobifyEcom.Contracts.Errors;

internal static partial class ErrorCatalog
{
	internal static readonly ErrorResponseDefinition SkillNotFound = new(
		Id: "SKILL_NOT_FOUND",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "The requested skill could not be found.",
		Details: []
	);

	internal static readonly ErrorResponseDefinition SkillAlreadyExists = new(
		Id: "SKILL_ALREADY_EXISTS",
		HttpStatus: StatusCodes.Status409Conflict,
		Title: "This skill already exists for the worker.",
		Details: []
	);

	internal static readonly ErrorResponseDefinition VerificationNotFound = new(
		Id: "VERIFICATION_NOT_FOUND",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "Verification record not found for this skill.",
		Details: []
	);

	internal static readonly ErrorResponseDefinition TagRequired = new(
		Id: "SKILL_TAG_REQUIRED",
		HttpStatus: StatusCodes.Status400BadRequest,
		Title: "At least one tag is required for a skill.",
		Details: []
	);

	internal static readonly ErrorResponseDefinition TagMaxCountExceeded = new(
		Id: "SKILL_TAG_MAX_EXCEEDED",
		HttpStatus: StatusCodes.Status400BadRequest,
		Title: "A skill cannot have more than the allowed number of tags.",
		Details: []
	);
}
