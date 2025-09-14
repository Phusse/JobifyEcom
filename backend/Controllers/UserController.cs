using JobifyEcom.Contracts.Routes;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.Users;
using JobifyEcom.Enums;
using JobifyEcom.Extensions;
using JobifyEcom.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobifyEcom.Controllers;

/// <summary>
/// Manages user operations: profile retrieval/update, account lock/unlock, password resets, email confirmation, and deletion.
/// </summary>
/// <param name="userService">Service for user-related operations.</param>
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
	private readonly IUserService _userService = userService;

	/// <summary>
	/// Retrieves the profile of the currently authenticated user.
	/// </summary>
	/// <remarks>
	/// Use this endpoint to fetch the current user's profile, including roles, permissions, and metadata.
	/// Requires the user to be authenticated. No parameters are needed in the request.
	/// The response contains all profile details relevant to the authenticated session.
	/// </remarks>
	/// <returns>The authenticated user's profile information.</returns>
	/// <response code="200">Successfully retrieved the user profile.</response>
	/// <response code="401">User is not authenticated.</response>
	[ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
	[Authorize]
	[HttpGet(ApiRoutes.User.Get.Me)]
	public async Task<IActionResult> GetCurrentUser()
	{
		ServiceResult<UserProfileResponse> result = await _userService.GetCurrentUserAsync();
		return Ok(result.MapToApiResponse());
	}

	/// <summary>
	/// Retrieves a user's public profile by their unique identifier.
	/// </summary>
	/// <remarks>
	/// Use this endpoint to fetch another user's profile. If the requester is an admin, additional details
	/// such as roles and account metadata will be included. Regular users will only see public information.
	/// Ensure the user ID provided is valid; otherwise, a 404 response is returned.
	/// </remarks>
	/// <param name="id">The unique identifier of the user whose profile you want to retrieve.</param>
	/// <returns>The requested user's profile information.</returns>
	/// <response code="200">User profile retrieved successfully.</response>
	/// <response code="401">User is not authenticated.</response>
	/// <response code="404">User not found.</response>
	[ProducesResponseType(typeof(ApiResponse<AdminUserProfileResponse>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	[Authorize]
	[HttpGet(ApiRoutes.User.Get.ById)]
	public async Task<IActionResult> GetUserById([FromRoute] Guid id)
	{
		ServiceResult<object> result = await _userService.GetUserByIdAsync(id);
		return Ok(result.MapToApiResponse());
	}

	/// <summary>
	/// Lists or searches users with optional filtering, sorting, and cursor-based pagination.
	/// </summary>
	/// <remarks>
	/// Use this endpoint to retrieve a paginated list of users. You can provide filter parameters
	/// such as name, email, role, or status to narrow the results. Pagination is cursor-based,
	/// meaning you can navigate through large datasets efficiently using the returned cursors.
	/// Only authenticated users can access this endpoint.
	/// </remarks>
	/// <param name="request">Pagination, filtering, and sorting parameters for querying users.</param>
	/// <returns>A paged list of user summaries matching the query.</returns>
	/// <response code="200">Users retrieved successfully.</response>
	[ProducesResponseType(typeof(ApiResponse<CursorPaginationResponse<UserProfileSummaryResponse>>), StatusCodes.Status200OK)]
	[Authorize]
	[HttpGet(ApiRoutes.Search.Get.Users)]
	[Obsolete("Deprecated, this is to reimplement in a new controller.")]
	public async Task<IActionResult> SearchUsers([FromQuery] CursorPaginationRequest<UserProfileFilterRequest> request)
	{
		ServiceResult<CursorPaginationResponse<UserProfileSummaryResponse>> result = await _userService.SearchUsersAsync(request);
		return Ok(result.MapToApiResponse());
	}

	/// <summary>
	/// Updates the profile of the currently authenticated user.
	/// </summary>
	/// <remarks>
	/// Allows the authenticated user to modify their own profile information such as name, email,
	/// and other editable fields. Ensure that the provided data meets validation rules. This
	/// endpoint requires the user to be logged in.
	/// </remarks>
	/// <param name="request">The details to update in the user's profile.</param>
	/// <returns>The updated user profile with the latest information.</returns>
	/// <response code="200">Profile updated successfully.</response>
	/// <response code="400">The request contains invalid or incomplete data.</response>
	/// <response code="401">User is not authenticated and cannot perform this action.</response>
	[ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
	[Authorize]
	[HttpPatch(ApiRoutes.User.Patch.Me)]
	public async Task<IActionResult> UpdateUser([FromBody] UserProfileUpdateRequest request)
	{
		ServiceResult<UserProfileResponse> result = await _userService.UpdateCurrentUserAsync(request);
		return Ok(result.MapToApiResponse());
	}

	/// <summary>
	/// Confirms a user's email address using a confirmation token via a link.
	/// </summary>
	/// <remarks>
	/// This endpoint is typically invoked when a user clicks the confirmation link sent to their email.
	/// It validates the token and activates the user's email if valid. Use GET for link-based confirmation
	/// (e.g., in emails) and PATCH for API-based confirmation.
	/// </remarks>
	/// <param name="request">Contains the user's email and the confirmation token.</param>
	/// <returns>Result indicating whether the email was successfully confirmed.</returns>
	/// <response code="200">Email confirmed successfully.</response>
	/// <response code="400">The confirmation token is invalid or has expired.</response>
	/// <response code="404">No user found with the provided email.</response>
	[HttpGet(ApiRoutes.User.Patch.ConfirmEmail)]
	public async Task<IActionResult> ConfirmEmailLink([FromQuery] EmailConfirmRequest request)
		=> await ConfirmEmailInternal(request);

	/// <summary>
	/// Confirms a user's email address using a confirmation token via API (PATCH).
	/// </summary>
	/// <remarks>
	/// This endpoint is intended for API clients or programmatic confirmation of a user's email.
	/// The provided token is validated, and the email is activated if valid. Use PATCH for direct
	/// API calls instead of link-based GET requests.
	/// </remarks>
	/// <param name="request">Contains the user's email and the confirmation token.</param>
	/// <returns>Result indicating whether the email was successfully confirmed.</returns>
	/// <response code="200">Email confirmed successfully.</response>
	/// <response code="400">The confirmation token is invalid or has expired.</response>
	/// <response code="404">No user found with the provided email.</response>
	[ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	[HttpPatch(ApiRoutes.User.Patch.ConfirmEmail)]
	public async Task<IActionResult> ConfirmEmail([FromQuery] EmailConfirmRequest request)
		=> await ConfirmEmailInternal(request);

	/// <summary>
	/// Handles the internal logic to confirm a user's email using a confirmation token.
	/// </summary>
	/// <remarks>
	/// This method is used by both link-based (GET) and API client (PATCH) endpoints to validate
	/// the confirmation token and activate the user's email. It should not be called directly by consumers.
	/// </remarks>
	/// <param name="request">Contains the user's email and the confirmation token.</param>
	/// <returns>Result indicating whether the email was successfully confirmed.</returns>
	private async Task<IActionResult> ConfirmEmailInternal(EmailConfirmRequest request)
	{
		ServiceResult<object> result = await _userService.ConfirmEmailAsync(request);
		return Ok(result.MapToApiResponse());
	}

	/// <summary>
	/// Locks a user account, preventing the user from logging in or accessing any resources.
	/// </summary>
	/// <remarks>
	/// Only administrators or super administrators can perform this action.
	/// This operation is useful for suspending accounts that violate policies or require temporary access restriction.
	/// </remarks>
	/// <param name="id">The unique identifier of the user account to lock.</param>
	/// <returns>Result of the lock operation indicating success or failure.</returns>
	/// <response code="200">User account locked successfully.</response>
	/// <response code="401">Requester is not authenticated.</response>
	/// <response code="403">Requester is not authorized to perform this operation.</response>
	/// <response code="404">User account not found.</response>
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	[Authorize(Roles = $"{nameof(SystemRole.Admin)}, {nameof(SystemRole.SuperAdmin)}")]
	[HttpPatch(ApiRoutes.User.Patch.Lock)]
	public async Task<IActionResult> LockUser([FromRoute] Guid id)
	{
		ServiceResult<object> result = await _userService.LockUserAsync(id);
		return Ok(result.MapToApiResponse());
	}

	/// <summary>
	/// Unlocks a previously locked user account, restoring login and access privileges.
	/// </summary>
	/// <remarks>
	/// Only administrators or super administrators can perform this action.
	/// Use this operation to reinstate access for users whose accounts were temporarily suspended or locked.
	/// </remarks>
	/// <param name="id">The unique identifier of the user account to unlock.</param>
	/// <returns>Result of the unlock operation indicating success or failure.</returns>
	/// <response code="200">User account unlocked successfully.</response>
	/// <response code="401">Requester is not authenticated.</response>
	/// <response code="403">Requester is not authorized to perform this operation.</response>
	/// <response code="404">User account not found.</response>
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	[Authorize(Roles = $"{nameof(SystemRole.Admin)}, {nameof(SystemRole.SuperAdmin)}")]
	[HttpPatch(ApiRoutes.User.Patch.Unlock)]
	public async Task<IActionResult> UnlockUser([FromRoute] Guid id)
	{
		ServiceResult<object> result = await _userService.UnlockUserAsync(id);
		return Ok(result.MapToApiResponse());
	}

	/// <summary>
	/// Initiates a password reset process for a user account.
	/// </summary>
	/// <remarks>
	/// Generates a password reset token that can be used to reset the account password.
	/// The token is valid for a specified duration and can be used to reset the password.
	/// </remarks>
	/// <param name="id">The unique identifier of the user account to reset the password for.</param>
	/// <returns>Result containing the password reset token and expiry (development only).</returns>
	/// <response code="200">Password reset token successfully generated.</response>
	/// <response code="404">User account not found.</response>
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	[HttpPost(ApiRoutes.User.Post.PasswordResetRequest)]
	public async Task<IActionResult> RequestPasswordReset([FromRoute] Guid id)
	{
		ServiceResult<object> result = await _userService.RequestPasswordResetAsync(id);
		return Ok(result.MapToApiResponse());
	}

	/// <summary>
	/// Resets a user's password using a valid password reset token.
	/// </summary>
	/// <remarks>
	/// The user must provide the reset token they received (via email or other channels) along with the new password.
	/// This endpoint validates the token and updates the user's password if the token is valid and has not expired.
	/// </remarks>
	/// <param name="id">The unique identifier of the user whose password is being reset.</param>
	/// <param name="request">Contains the password reset token and the new password.</param>
	/// <returns>Result indicating whether the password reset was successful.</returns>
	/// <response code="200">Password was reset successfully.</response>
	/// <response code="400">The reset token is invalid or has expired.</response>
	/// <response code="404">User account not found.</response>
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	[HttpPost(ApiRoutes.User.Post.PasswordResetConfirm)]
	public async Task<IActionResult> ResetPassword([FromRoute] Guid id, [FromBody] PasswordResetRequest request)
	{
		ServiceResult<object> result = await _userService.ResetPasswordAsync(id, request);
		return Ok(result.MapToApiResponse());
	}

	/// <summary>
	/// Deletes the account of the currently authenticated user.
	/// </summary>
	/// <remarks>
	/// Permanently removes the user's account and all associated data from the system.
	/// This operation requires the user to be authenticated and cannot be undone.
	/// After deletion, the user will no longer be able to log in.
	/// </remarks>
	/// <returns>Result indicating whether the account deletion was successful.</returns>
	/// <response code="200">Account deleted successfully.</response>
	/// <response code="401">User is not authenticated.</response>
	/// <response code="404">User account not found.</response>
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	[Authorize]
	[HttpDelete(ApiRoutes.User.Delete.Me)]
	public async Task<IActionResult> DeleteCurrentUser()
	{
		ServiceResult<object> result = await _userService.DeleteCurrentUserAsync();
		return Ok(result.MapToApiResponse());
	}

	/// <summary>
	/// Deletes a user account by its unique identifier (admin only).
	/// </summary>
	/// <remarks>
	/// This operation allows administrators to permanently remove a user and all associated data from the system.
	/// Only users with the <c>Admin</c> or <c>SuperAdmin</c> roles are authorized to perform this action.
	/// Attempting to delete a non-existent user will return a 404 response.
	/// </remarks>
	/// <param name="id">The unique identifier of the user to delete.</param>
	/// <returns>Result indicating whether the account deletion was successful.</returns>
	/// <response code="200">User account deleted successfully.</response>
	/// <response code="401">The requester is not authenticated.</response>
	/// <response code="403">The requester does not have permission to delete the user.</response>
	/// <response code="404">The specified user account was not found.</response>
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	[Authorize(Roles = $"{nameof(SystemRole.Admin)}, {nameof(SystemRole.SuperAdmin)}")]
	[HttpDelete(ApiRoutes.User.Delete.ById)]
	public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
	{
		ServiceResult<object> result = await _userService.DeleteUserAsync(id);
		return Ok(result.MapToApiResponse());
	}
}
