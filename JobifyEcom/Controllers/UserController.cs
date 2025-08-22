using JobifyEcom.Contracts;
using JobifyEcom.DTOs;
using JobifyEcom.DTOs.User;
using JobifyEcom.Enums;
using JobifyEcom.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobifyEcom.Controllers;

[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
	private readonly IUserService _userService = userService;

	[ProducesResponseType(typeof(ApiResponse<ProfileResponse>), StatusCodes.Status200OK)]
	[Authorize]
	[HttpGet(ApiRoutes.Users.Get.Me)]
	public async Task<IActionResult> GetCurrentUser()
	{
		ServiceResult<ProfileResponse> result = await _userService.GetCurrentUserAsync();
		return Ok(ApiResponse<ProfileResponse>.Ok(result.Data, result.Message, result.Errors));
	}

	[Authorize]
	[HttpGet(ApiRoutes.Users.Get.ById)]
	public async Task<IActionResult> GetUserById([FromRoute] Guid id)
	{
		var result = await _userService.GetUserByIdAsync(id);
		return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
	}

	// GET: List/Search users
	[HttpGet(ApiRoutes.Users.Get.List)]
	public async Task<IActionResult> SearchUsers([FromQuery] CursorPaginationRequest<ProfileFilterRequest> request)
	{
		var result = await _userService.SearchUsersAsync(request);
		return Ok(ApiResponse<CursorPaginationResponse<ProfileSummaryResponse>>.Ok(result.Data, result.Message, result.Errors));
	}

	// PATCH: Update user
	[HttpPatch(ApiRoutes.Users.Patch.Update)]
	public async Task<IActionResult> UpdateUser([FromBody] ProfileUpdateRequest request)
	{
		var result = await _userService.UpdateCurrentUserAsync(request);
		return Ok(ApiResponse<ProfileResponse>.Ok(result.Data, result.Message, result.Errors));
	}

	[ApiExplorerSettings(IgnoreApi = true)]
	[HttpGet(ApiRoutes.Users.Patch.ConfirmEmail)]
	public async Task<IActionResult> ConfirmEmailLink([FromQuery] EmailConfirmRequest request)
		=> await ConfirmEmailInternal(request);

	[ProducesResponseType(typeof(ApiResponse<ProfileResponse>), StatusCodes.Status200OK)]
	[HttpPatch(ApiRoutes.Users.Patch.ConfirmEmail)]
	public async Task<IActionResult> ConfirmEmail([FromQuery] EmailConfirmRequest request)
		=> await ConfirmEmailInternal(request);

	private async Task<IActionResult> ConfirmEmailInternal(EmailConfirmRequest request)
	{
		ServiceResult<object> result = await _userService.ConfirmEmailAsync(request);
		return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
	}

	// PATCH: Lock user
	[Authorize(Roles = $"{nameof(SystemRole.Admin)}, {nameof(SystemRole.SuperAdmin)}")]
	[HttpPatch(ApiRoutes.Users.Patch.Lock)]
	public async Task<IActionResult> LockUser([FromRoute] Guid id)
	{
		var result = await _userService.LockUserAsync(id);
		return Ok(ApiResponse<object>.Ok(result.Data, result.Message, result.Errors));
	}

	// PATCH: Unlock user
	[Authorize(Roles = $"{nameof(SystemRole.Admin)}, {nameof(SystemRole.SuperAdmin)}")]
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

	[HttpDelete(ApiRoutes.Users.Delete.Me)]
	public async Task<IActionResult> DeleteCurrentUser()
	{
		var result = await _userService.DeleteCurrentUserAsync();
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
