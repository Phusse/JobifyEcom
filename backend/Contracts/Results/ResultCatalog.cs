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

	public static readonly ResultResponseDefinition UserRetrieved = new(
		Id: "USER_RETRIEVED",
		Title: "User retrieved successfully.",
		Details: []
	);

	public static readonly ResultResponseDefinition CurrentUserRetrieved = new(
		Id: "USER_CURRENT_RETRIEVED",
		Title: "Your account details have been retrieved successfully.",
		Details: []
	);

	public static readonly ResultResponseDefinition UserDeleted = new(
		Id: "USER_DELETED",
		Title: "The account was successfully deleted.",
		Details: []
	);

	public static readonly ResultResponseDefinition CurrentUserDeleted = new(
		Id: "USER_CURRENT_DELETED",
		Title: "Your account has been deleted. We're sorry to see you go.",
		Details: []
	);

	public static readonly ResultResponseDefinition EmailConfirmed = new(
		Id: "USER_EMAIL_CONFIRMED",
		Title: "Success! Your email address has been confirmed.",
		Details: []
	);

	public static readonly ResultResponseDefinition EmailAlreadyConfirmed = new(
		Id: "USER_EMAIL_ALREADY_CONFIRMED",
		Title: "Your email is already confirmed. You can now sign in.",
		Details: []
	);

	public static readonly ResultResponseDefinition UserLocked = new(
		Id: "USER_LOCKED",
		Title: "User account has been locked.",
		Details: []
	);

	public static readonly ResultResponseDefinition UserAlreadyLocked = new(
		Id: "USER_ALREADY_LOCKED",
		Title: "This user account is already locked.",
		Details: []
	);

	public static readonly ResultResponseDefinition UserUnlocked = new(
		Id: "USER_UNLOCKED",
		Title: "User account has been unlocked.",
		Details: []
	);

	public static readonly ResultResponseDefinition UserAlreadyUnlocked = new(
		Id: "USER_ALREADY_UNLOCKED",
		Title: "This user account is not locked.",
		Details: []
	);

	public static readonly ResultResponseDefinition PasswordResetRequested = new(
		Id: "USER_PASSWORD_RESET_REQUESTED",
		Title: "A password reset link has been sent to your email address.",
		Details: []
	);

	public static readonly ResultResponseDefinition PasswordResetSuccessful = new(
		Id: "USER_PASSWORD_RESET_SUCCESSFUL",
		Title: "Your password has been updated. You can now sign in with your new password.",
		Details: []
	);

	public static readonly ResultResponseDefinition UserProfileUpdated = new(
		Id: "USER_PROFILE_UPDATED",
		Title: "User profile updated successfully.",
		Details: []
	);

	public static readonly ResultResponseDefinition LoginSuccessful = new(
	Id: "AUTH_LOGIN_SUCCESS",
	Title: "Signed in successfully.",
	Details: []
);

	public static readonly ResultResponseDefinition RefreshSuccessful = new(
		Id: "AUTH_REFRESH_SUCCESS",
		Title: "Your session has been renewed. You are still signed in.",
		Details: []
	);

	public static readonly ResultResponseDefinition LogoutSuccessful = new(
		Id: "AUTH_LOGOUT_SUCCESS",
		Title: "You have been signed out.",
		Details: []
	);

	public static readonly ResultResponseDefinition RegistrationSuccessful = new(
		Id: "AUTH_REGISTRATION_SUCCESS",
		Title: "Registration successful! Please check your email for a confirmation link to activate your account.",
		Details: []
	);
}
