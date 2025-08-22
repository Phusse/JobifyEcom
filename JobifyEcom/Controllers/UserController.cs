using JobifyEcom.Contracts;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.User;
using JobifyEcom.Enums;
using JobifyEcom.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobifyEcom.Controllers;

/// <summary>
/// Handles user management operations including profile retrieval, updates, account locking/unlocking, password resets, and deletion.
/// </summary>
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
	private readonly IUserService _userService = userService;

	/// <summary>
	/// Retrieves the profile of the currently authenticated user.
	/// </summary>
	/// <remarks>
	/// Requires authentication. Returns the user's profile details including roles and metadata.
	/// </remarks>
	/// <returns>The current user's profile.</returns>
	/// <response code="200">Profile retrieved successfully.</response>
	/// <response code="401">User is not authenticated.</response>
	[ProducesResponseType(typeof(ApiResponse<ProfileResponse>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
	[Authorize]
	[HttpGet(ApiRoutes.Users.Get.Me)]
	public async Task<IActionResult> GetCurrentUser()
	{
		ServiceResult<ProfileResponse> result = await _userService.GetCurrentUserAsync();
		return Ok(ApiResponse<ProfileResponse>.Ok(result.Data, result.Message, result.Errors));
	}

	/// <summary>
	/// Retrieves a user's public profile by their unique identifier.
	/// </summary>
	/// <remarks>
	/// Returns more details if the requester is an admin.
	/// </remarks>
	/// <param name="id">The unique identifier of the user.</param>
	/// <returns>The user's profile.</returns>
	/// <response code="200">User profile retrieved successfully.</response>
	/// <response code="401">User is not authenticated.</response>
	/// <response code="404">User not found.</response>
	[ProducesResponseType(typeof(ApiResponse<AdminProfileResponse>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	[Authorize]
	[HttpGet(ApiRoutes.Users.Get.ById)]
	public async Task<IActionResult> GetUserById([FromRoute] Guid id)
	{
		ServiceResult<object> result = await _userService.GetUserByIdAsync(id);
		return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
	}

	/// <summary>
	/// Lists or searches users with optional filtering, sorting, and cursor-based pagination.
	/// </summary>
	/// <param name="request">Pagination and filter parameters.</param>
	/// <returns>Paged list of user summaries.</returns>
	/// <response code="200">Users retrieved successfully.</response>
	[ProducesResponseType(typeof(ApiResponse<CursorPaginationResponse<ProfileSummaryResponse>>), StatusCodes.Status200OK)]
	[HttpGet(ApiRoutes.Users.Get.List)]
	[Authorize]
	public async Task<IActionResult> SearchUsers([FromQuery] CursorPaginationRequest<ProfileFilterRequest> request)
	{
		ServiceResult<CursorPaginationResponse<ProfileSummaryResponse>> result = await _userService.SearchUsersAsync(request);
		return Ok(ApiResponse<CursorPaginationResponse<ProfileSummaryResponse>>.Ok(result.Data, result.Message, result.Errors));
	}

	/// <summary>
	/// Updates the profile of the currently authenticated user.
	/// </summary>
	/// <param name="request">Profile update details.</param>
	/// <returns>The updated user profile.</returns>
	/// <response code="200">Profile updated successfully.</response>
	/// <response code="400">Invalid update request.</response>
	/// <response code="401">User is not authenticated.</response>
	[ProducesResponseType(typeof(ApiResponse<ProfileResponse>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
	[Authorize]
	[HttpPatch(ApiRoutes.Users.Patch.Update)]
	public async Task<IActionResult> UpdateUser([FromBody] ProfileUpdateRequest request)
	{
		ServiceResult<ProfileResponse> result = await _userService.UpdateCurrentUserAsync(request);
		return Ok(ApiResponse<ProfileResponse>.Ok(result.Data, result.Message, result.Errors));
	}

	/// <summary>
	/// Confirms a user's email address using a confirmation token (GET for link-based confirmation).
	/// </summary>
	/// <param name="request">Email and confirmation token.</param>
	/// <returns>Confirmation result.</returns>
	/// <response code="200">Email confirmed successfully.</response>
	/// <response code="400">Invalid or expired confirmation link.</response>
	/// <response code="404">User not found.</response>
	[HttpGet(ApiRoutes.Users.Patch.ConfirmEmail)]
	public async Task<IActionResult> ConfirmEmailLink([FromQuery] EmailConfirmRequest request)
		=> await ConfirmEmailInternal(request);

	/// <summary>
	/// Confirms a user's email address using a confirmation token (PATCH for API clients).
	/// </summary>
	/// <param name="request">Email and confirmation token.</param>
	/// <returns>Confirmation result.</returns>
	/// <response code="200">Email confirmed successfully.</response>
	/// <response code="400">Invalid or expired confirmation link.</response>
	/// <response code="404">User not found.</response>
	[ProducesResponseType(typeof(ApiResponse<ProfileResponse>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	[HttpPatch(ApiRoutes.Users.Patch.ConfirmEmail)]
	public async Task<IActionResult> ConfirmEmail([FromQuery] EmailConfirmRequest request)
		=> await ConfirmEmailInternal(request);

	/// <summary>
	/// Confirms a user's email address using a confirmation token.
	/// </summary>
	/// <param name="request">Email and confirmation token.</param>
	/// <returns>Confirmation result.</returns>
	private async Task<IActionResult> ConfirmEmailInternal(EmailConfirmRequest request)
	{
		ServiceResult<object> result = await _userService.ConfirmEmailAsync(request);
		return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
	}

	/// <summary>
	/// Locks a user account, preventing login and access.
	/// </summary>
	/// <param name="id">The unique identifier of the user to lock.</param>
	/// <returns>Lock operation result.</returns>
	/// <response code="200">User account locked successfully.</response>
	/// <response code="401">User is not authenticated.</response>
	/// <response code="403">Operation not permitted.</response>
	/// <response code="404">User not found.</response>
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	[Authorize(Roles = $"{nameof(SystemRole.Admin)}, {nameof(SystemRole.SuperAdmin)}")]
	[HttpPatch(ApiRoutes.Users.Patch.Lock)]
	public async Task<IActionResult> LockUser([FromRoute] Guid id)
	{
		ServiceResult<object> result = await _userService.LockUserAsync(id);
		return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
	}

	/// <summary>
	/// Unlocks a previously locked user account.
	/// </summary>
	/// <param name="id">The unique identifier of the user to unlock.</param>
	/// <returns>Unlock operation result.</returns>
	/// <response code="200">User account unlocked successfully.</response>
	/// <response code="401">User is not authenticated.</response>
	/// <response code="403">Operation not permitted.</response>
	/// <response code="404">User not found.</response>
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	[Authorize(Roles = $"{nameof(SystemRole.Admin)}, {nameof(SystemRole.SuperAdmin)}")]
	[HttpPatch(ApiRoutes.Users.Patch.Unlock)]
	public async Task<IActionResult> UnlockUser([FromRoute] Guid id)
	{
		ServiceResult<object> result = await _userService.UnlockUserAsync(id);
		return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
	}

	/// <summary>
	/// Requests a password reset for a user account.
	/// </summary>
	/// <param name="id">The unique identifier of the user.</param>
	/// <returns>Password reset token and expiry (for development).</returns>
	/// <response code="200">Password reset token generated.</response>
	/// <response code="404">User not found.</response>
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	[HttpPost(ApiRoutes.Users.Post.PasswordResetRequest)]
	public async Task<IActionResult> RequestPasswordReset([FromRoute] Guid id)
	{
		ServiceResult<object> result = await _userService.RequestPasswordResetAsync(id);
		return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
	}

	/// <summary>
	/// Confirms a password reset using a valid reset token.
	/// </summary>
	/// <param name="id">The unique identifier of the user.</param>
	/// <param name="request">Password reset details including token and new password.</param>
	/// <returns>Password reset result.</returns>
	/// <response code="200">Password reset successfully.</response>
	/// <response code="400">Invalid or expired reset token.</response>
	/// <response code="404">User not found.</response>
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	[HttpPost(ApiRoutes.Users.Post.PasswordResetConfirm)]
	public async Task<IActionResult> ResetPassword([FromRoute] Guid id, [FromBody] PasswordResetRequest request)
	{
		ServiceResult<object> result = await _userService.ResetPasswordAsync(id, request);
		return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
	}

	/// <summary>
	/// Deletes the currently authenticated user's account.
	/// </summary>
	/// <returns>Account deletion result.</returns>
	/// <response code="200">Account deleted successfully.</response>
	/// <response code="401">User is not authenticated.</response>
	/// <response code="404">User not found.</response>
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	[Authorize]
	[HttpDelete(ApiRoutes.Users.Delete.Me)]
	public async Task<IActionResult> DeleteCurrentUser()
	{
		ServiceResult<object> result = await _userService.DeleteCurrentUserAsync();
		return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
	}

	/// <summary>
	/// Deletes a user account by unique identifier (admin only).
	/// </summary>
	/// <param name="id">The unique identifier of the user to delete.</param>
	/// <returns>Account deletion result.</returns>
	/// <response code="200">Account deleted successfully.</response>
	/// <response code="401">User is not authenticated.</response>
	/// <response code="403">Operation not permitted.</response>
	/// <response code="404">User not found.</response>
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	[Authorize(Roles = $"{nameof(SystemRole.Admin)}, {nameof(SystemRole.SuperAdmin)}")]
	[HttpDelete(ApiRoutes.Users.Delete.ById)]
	public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
	{
		ServiceResult<object> result = await _userService.DeleteUserAsync(id);
		return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
	}
}
