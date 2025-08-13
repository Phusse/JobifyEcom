using JobifyEcom.Contracts;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.User;
using JobifyEcom.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobifyEcom.Controllers;

[Authorize]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
	private readonly IUserService _userService = userService;

	// GET: Current authenticated user
	[HttpGet(ApiRoutes.Users.Get.Me)]
	public async Task<IActionResult> GetCurrentUser()
	{
		var result = await _userService.GetCurrentUserAsync();
		return Ok(ApiResponse<ProfileResponse>.Ok(result.Data, result.Message, result.Errors));
	}

	// GET: User by ID
	[HttpGet(ApiRoutes.Users.Get.ById)]
	public async Task<IActionResult> GetUserById([FromRoute] Guid id)
	{
		var result = await _userService.GetUserByIdAsync(id);
		return Ok(ApiResponse<ProfileResponse>.Ok(result.Data, result.Message, result.Errors));
	}

	// GET: List/Search users
	[HttpGet(ApiRoutes.Users.Get.List)]
	public async Task<IActionResult> SearchUsers([FromQuery] CursorPaginationRequest<ProfileFilterRequest> request)
	{
		var result = await _userService.SearchUsersAsync(request);
		return Ok(ApiResponse<CursorPaginationResponse<ProfileSummaryResponse>>.Ok(result.Data, result.Message, result.Errors));
	}

	// PATCH: Update user

	[ProducesResponseType(typeof(ApiResponse<ProfileResponse>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<ProfileResponse>), StatusCodes.Status200OK)]
	[HttpPatch(ApiRoutes.Users.Patch.Update)]
	public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] ProfileUpdateRequest request)
	{
		var result = await _userService.UpdateUserAsync(id, request);
		return Ok(ApiResponse<ProfileResponse>.Ok(result.Data, result.Message, result.Errors));
	}

	// PATCH: Confirm email
	[HttpPatch(ApiRoutes.Users.Patch.ConfirmEmail)]
	public async Task<IActionResult> ConfirmEmail([FromBody] EmailConfirmRequest request)
	{
		var result = await _userService.ConfirmEmailAsync(request);
		return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
	}

	// PATCH: Lock user
	[HttpPatch(ApiRoutes.Users.Patch.Lock)]
	public async Task<IActionResult> LockUser([FromRoute] Guid id)
	{
		var result = await _userService.LockUserAsync(id);
		return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
	}

	// PATCH: Unlock user
	[HttpPatch(ApiRoutes.Users.Patch.Unlock)]
	public async Task<IActionResult> UnlockUser([FromRoute] Guid id)
	{
		var result = await _userService.UnlockUserAsync(id);
		return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
	}

	// POST: Request password reset
	[HttpPost(ApiRoutes.Users.Post.PasswordResetRequest)]
	public async Task<IActionResult> RequestPasswordReset([FromRoute] Guid id)
	{
		var result = await _userService.RequestPasswordResetAsync(id);
		return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
	}

	// POST: Confirm password reset
	[HttpPost(ApiRoutes.Users.Post.PasswordResetConfirm)]
	public async Task<IActionResult> ResetPassword([FromRoute] Guid id, [FromBody] PasswordResetRequest request)
	{
		var result = await _userService.ResetPasswordAsync(id, request);
		return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
	}

	// DELETE: Delete user
	[HttpDelete(ApiRoutes.Users.Delete.ById)]
	public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
	{
		var result = await _userService.DeleteUserAsync(id);
		return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
	}
}
