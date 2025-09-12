using JobifyEcom.Exceptions;

namespace JobifyEcom.Contracts.Errors;

internal static partial class ErrorCatalog
{
	internal static readonly ErrorDefinition SkillNotFound = new(
		Code: "SKILL_NOT_FOUND",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "The requested skill could not be found.",
		Details: []
	);

	internal static readonly ErrorDefinition VerificationNotFound = new(
		Code: "VERIFICATION_NOT_FOUND",
		HttpStatus: StatusCodes.Status404NotFound,
		Title: "Verification record not found for this skill.",
		Details: []
	);

	internal static readonly ErrorDefinition TagRequired = new(
		Code: "SKILL_TAG_REQUIRED",
		HttpStatus: StatusCodes.Status400BadRequest,
		Title: "At least one tag is required for a skill.",
		Details: []
	);
}
