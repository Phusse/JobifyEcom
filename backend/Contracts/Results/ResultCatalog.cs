using JobifyEcom.Contracts.Responses;

namespace JobifyEcom.Contracts.Results;

/// <summary>
/// Central registry of all standardized application results (non-error outcomes).
/// Each entry provides a unique code, a user-facing title, and optional details.
/// Organized into <c>partial</c> files by domain for easier maintenance,
/// and can be used to represent successful or informational outcomes in <c>ServiceResult</c>.
/// </summary>
internal static partial class ResultCatalog
{
	public static readonly ResultResponseDefinition AllEnumsRetrieved = new(
		Id: "METADATA_ENUMS_RETRIEVED",
		Title: "All enums retrieved successfully.",
		Details: []
	);

	public static readonly ResultResponseDefinition EnumRetrieved = new(
		Id: "METADATA_ENUM_RETRIEVED",
		Title: "Enum retrieved successfully.",
		Details: []
	);

	internal static readonly ResultResponseDefinition EnumNotFound = new(
		Id: "METADATA_ENUM_NOT_FOUND",
		Title: $"The requested enum could not be found.",
		Details: ["Please check the list of available enums."]
	);

	public static readonly ResultResponseDefinition SkillAdded = new(
		Id: "WORKER_SKILL_ADDED",
		Title: "Skill submitted successfully and is pending verification.",
		Details: []
	);

	public static readonly ResultResponseDefinition SkillRemoved = new(
		Id: "WORKER_SKILL_REMOVED",
		Title: "Skill removed successfully.",
		Details: []
	);

	public static readonly ResultResponseDefinition SkillRetrieved = new(
		Id: "WORKER_SKILL_RETRIEVED",
		Title: "Skill retrieved successfully.",
		Details: []
	);

	public static readonly ResultResponseDefinition UsersFound = new(
		Id: "SEARCH_USERS_FOUND",
		Title: "Users retrieved successfully.",
		Details: []
	);

	public static readonly ResultResponseDefinition UsersNotFound = new(
		Id: "SEARCH_USERS_NOT_FOUND",
		Title: "No users found.",
		Details: ["Try adjusting your search criteria or filters."]
	);

	public static readonly ResultResponseDefinition MaxCursorDepthReached = new(
		Id: "SEARCH_MAX_CURSOR_DEPTH_REACHED",
		Title: "No more results.",
		Details: ["The maximum cursor depth for pagination has been reached."]
	);

	public static readonly ResultResponseDefinition WorkerProfileCreated = new(
		Id: "WORKER_PROFILE_CREATED",
		Title: "Worker profile created successfully.",
		Details: []
	);

	public static readonly ResultResponseDefinition WorkerProfileDeleted = new(
		Id: "WORKER_PROFILE_DELETED",
		Title: "Worker profile deleted successfully.",
		Details: []
	);

	public static readonly ResultResponseDefinition WorkerProfileRetrieved = new(
		Id: "WORKER_PROFILE_RETRIEVED",
		Title: "Worker profile retrieved successfully.",
		Details: []
	);

	public static readonly ResultResponseDefinition JobCreated = new(
		Id: "JOB_CREATED",
		Title: "Job created successfully.",
		Details: []
	);

	public static readonly ResultResponseDefinition JobRetrieved = new(
		Id: "JOB_RETRIEVED",
		Title: "Job retrieved successfully.",
		Details: []
	);

	public static readonly ResultResponseDefinition JobUpdated = new(
		Id: "JOB_UPDATED",
		Title: "Job updated successfully.",
		Details: []
	);

	public static readonly ResultResponseDefinition JobDeleted = new(
		Id: "JOB_DELETED",
		Title: "Job deleted successfully.",
		Details: []
	);

	public static readonly ResultResponseDefinition JobApplicationCreated = new(
		Id: "JOB_APPLICATION_CREATED",
		Title: "Your application has been submitted successfully.",
		Details: []
	);

	public static readonly ResultResponseDefinition JobApplicationRetrieved = new(
		Id: "JOB_APPLICATION_RETRIEVED",
		Title: "Job application retrieved successfully.",
		Details: []
	);

	public static readonly ResultResponseDefinition JobApplicationStatusUpdated = new(
		Id: "JOB_APPLICATION_STATUS_UPDATED",
		Title: "The application status has been updated successfully.",
		Details: []
	);

	public static readonly ResultResponseDefinition JobApplicationStatusAlreadySet = new(
		Id: "JOB_APPLICATION_STATUS_ALREADY_SET",
		Title: "The application already has this status.",
		Details: []
	);
}
