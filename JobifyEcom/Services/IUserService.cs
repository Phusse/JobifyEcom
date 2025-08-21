using JobifyEcom.DTOs;
using JobifyEcom.DTOs.User;

namespace JobifyEcom.Services;

/// <summary>
/// Defines operations for managing and retrieving user accounts.
/// </summary>
public interface IUserService
{
	/// <summary>
	/// Retrieves the profile of the currently authenticated user.
	/// </summary>
	/// <returns>A service result containing the authenticated user's profile.</returns>
	Task<ServiceResult<ProfileResponse>> GetCurrentUserAsync();

	/// <summary>
	/// Retrieves the public profile of a specific user by their unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the user.</param>
	/// <returns>A service result containing the user's public profile.</returns>
	Task<ServiceResult<ProfileResponse>> GetUserByIdAsync(Guid id);

	/// <summary>
	/// Searches and lists users using pagination and optional filtering.
	/// </summary>
	/// <param name="request">The pagination and filter criteria.</param>
	/// <returns>A paginated list of user summaries.</returns>
	Task<ServiceResult<CursorPaginationResponse<ProfileSummaryResponse>>> SearchUsersAsync(CursorPaginationRequest<ProfileFilterRequest> request);

	/// <summary>
	/// Updates the profile information of a specific user.
	/// </summary>
	/// <param name="request">The new profile values to update.</param>
	/// <returns>The updated user profile.</returns>
	Task<ServiceResult<ProfileResponse>> UpdateCurrentUserAsync(ProfileUpdateRequest request);

	/// <summary>
	/// Confirms a user's email address using a verification token.
	/// </summary>
	/// <param name="request">The email confirmation details.</param>
	/// <returns>A result indicating success or failure.</returns>
	Task<ServiceResult<object>> ConfirmEmailAsync(EmailConfirmRequest request);

	/// <summary>
	/// Locks a user account, preventing login.
	/// </summary>
	/// <param name="id">The unique identifier of the user to lock.</param>
	/// <returns>A result indicating success or failure.</returns>
	Task<ServiceResult<object>> LockUserAsync(Guid id);

	/// <summary>
	/// Unlocks a previously locked user account.
	/// </summary>
	/// <param name="id">The unique identifier of the user to unlock.</param>
	/// <returns>A result indicating success or failure.</returns>
	Task<ServiceResult<object>> UnlockUserAsync(Guid id);

	/// <summary>
	/// Requests a password reset for a user account.
	/// </summary>
	/// <param name="id">The unique identifier of the user requesting the reset.</param>
	/// <returns>A result indicating success or failure.</returns>
	Task<ServiceResult<object>> RequestPasswordResetAsync(Guid id);

	/// <summary>
	/// Resets a user's password using a reset token.
	/// </summary>
	/// <param name="id">The unique identifier of the user.</param>
	/// <param name="request">The password reset details.</param>
	/// <returns>A result indicating success or failure.</returns>
	Task<ServiceResult<object>> ResetPasswordAsync(Guid id, PasswordResetRequest request);

	/// <summary>
	/// Deletes the currently authenticated user's account.
	/// </summary>
	/// <returns>A result indicating success or failure.</returns>
	Task<ServiceResult<object>> DeleteCurrentUserAsync();

	/// <summary>
	/// Permanently deletes a user account.
	/// </summary>
	/// <param name="id">The unique identifier of the user to delete.</param>
	/// <returns>A result indicating success or failure.</returns>
	Task<ServiceResult<object>> DeleteUserAsync(Guid id);
}
